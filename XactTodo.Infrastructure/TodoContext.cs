using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XactTodo.Domain;
using XactTodo.Domain.AggregatesModel.MatterAggregate;
using XactTodo.Domain.AggregatesModel.MatterTagAggregate;
using XactTodo.Domain.AggregatesModel.TeamAggregate;
using XactTodo.Domain.AggregatesModel.UserAggregate;
using XactTodo.Domain.SeedWork;
using XactTodo.Infrastructure.Extensions;
using XactTodo.Infrastructure.EntityConfigurations;
using XactTodo.Domain.AggregatesModel.IdentityAggregate;
using XactTodo.Security;
using XactTodo.Security.Session;

namespace XactTodo.Infrastructure
{
    public class TodoContext : DbContext, IUnitOfWork
    {
        private const string column_IsDeleted = "IsDeleted";

        public IClaimsSession Session { get; }

        public int InstanceId { get; }
        public TodoContext(DbContextOptions<TodoContext> options, IClaimsSession session) : base(options)
        {
            this.InstanceId = new Random(Environment.TickCount).Next();
            Session = session;
        }

        public DbSet<Identity> Identities {get;set;}
        public DbSet<User> Users { get; set; }
        public DbSet<Matter> Matters { get; set; }
        public DbSet<MatterTag> MatterTags { get; set; }
        public DbSet<Team> Teams { get; set; }

        //public DbSet<Member> Members { get; set; } //不是根实体不用声明变量
        //public DbSet<> s { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //将所有decimal类型的字段都默认设置为decimal(18,2)，通过后面的ApplyConfiguration()方法可个别再调整
            //但是需要留意：值对象对应的字段配置不了，因为那已经是实体类属性的属性了，需要通过EntityTypeBuilder.OwnsOne().Property().HasColumnType()来配置
            var entityTypes = modelBuilder.Model.GetEntityTypes().Where(p => !p.ClrType.IsSubclassOf(typeof(ValueObject))).ToArray(); //这里一定要排除掉值类型的EntityType，否则，嘿嘿，够喝上一壶的
            foreach (var pb in entityTypes
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal))
                .Select(p => modelBuilder.Entity(p.DeclaringEntityType.ClrType).Property(p.Name)))
            {
                pb.HasColumnType("decimal(18,2)");
            }

            modelBuilder.ApplyConfiguration(new MatterEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new EvolvementEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MatterTagEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TeamEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MemberEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());

            modelBuilder.RemovePluralizingTableNameConvention();
            var typeContext = this.GetType();
            foreach (var prop in typeContext.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.PropertyType.IsGenericType && prop.PropertyType.Name == typeof(DbSet<>).Name)
                {
                    var typeEntity = prop.PropertyType.GetGenericArguments().FirstOrDefault();
                    ConfigEntityProperty(modelBuilder, typeEntity);
                }
            }

            base.OnModelCreating(modelBuilder);
        }

        private void ConfigEntityProperty(ModelBuilder modelBuilder, Type typeEntity)
        {
            //如果实现了ICreationAudited接口
            if (typeEntity.GetInterface(nameof(ICreationAudited)) != null)
            {
                modelBuilder.Entity(typeEntity).Property(nameof(ICreationAudited.CreationTime)).HasDefaultValueSql(this.Database.IsMySql() ? "sysdate()" : "getdate()");
            }
            //如果实现了软删除接口
            if (typeEntity.GetInterface(nameof(ISoftDelete)) != null)
            {
                //添加软删除列
                modelBuilder.Entity(typeEntity).Property<bool>(column_IsDeleted).HasDefaultValue(false);
                //对软删除列的查询过滤
                ParameterExpression parameter = Expression.Parameter(typeEntity, "p");
                var methodEFProperty = typeof(EF).GetMethod(nameof(EF.Property));
                methodEFProperty = methodEFProperty.MakeGenericMethod(typeof(bool));
                var member = Expression.Call(methodEFProperty, parameter, Expression.Constant(column_IsDeleted));
                var methodEquals = typeof(bool).GetMethod("Equals", new[] { typeof(bool) });
                var constFalse = Expression.Constant(false, typeof(bool));
                var expr = Expression.Lambda(Expression.Equal(member, constFalse), parameter);
                modelBuilder.Entity(typeEntity).HasQueryFilter(expr);
            }
            var nullableTypeName = typeof(Nullable<>).Name;
            var navProperties = typeEntity.GetProperties().Where(p => p.PropertyType.IsGenericType && p.PropertyType.Name!=nullableTypeName);
            foreach(var property in navProperties)
            {
                var genericType = property.PropertyType.GetGenericArguments().First();
                if (typeof(ICollection<>).MakeGenericType(genericType).IsAssignableFrom(property.PropertyType))
                {
                    ConfigEntityProperty(modelBuilder, genericType);
                }
            }
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaving()
        {
            //Session.VerifyLoggedin();//不能在这提前校验，因为有些类是无需用户会话信息的，例如登录时记录的身份认证信息
            foreach (var entry in ChangeTracker.Entries().Where(p => p.State != EntityState.Unchanged))
            {
                var typeEntity = entry.Entity.GetType();
                switch (entry.State)
                {
                    case EntityState.Added:
                        if (typeEntity.GetInterface(nameof(ICreationAudited)) != null)
                        {
                            Session.VerifyLoggedin();
                            //只需要给创建人字段赋值，创建时间字段已设置数据库默认值
                            entry.CurrentValues[nameof(ICreationAudited.CreatorUserId)] = Session.UserId;
                        }
                        /* IsDeleted列已设置数据库默认值为0
                        if (typeEntity.GetInterface(nameof(ISoftDelete)) != null)
                            entry.CurrentValues[column_IsDeleted] = false;
                        */
                        break;
                    case EntityState.Modified:
                        if (typeEntity.GetInterface(nameof(IModificationAudited)) != null)
                        {
                            Session.VerifyLoggedin();
                            entry.CurrentValues[nameof(IModificationAudited.LastModifierUserId)] = Session.UserId;
                            entry.CurrentValues[nameof(IModificationAudited.LastModificationTime)] = DateTime.Now;
                        }
                        break;
                    case EntityState.Deleted:
                        //如果实现了ISoftDelete接口，则强制设置为 "Modified" 状态，然后设置软删除字段值为true
                        if (typeEntity.GetInterface(nameof(ISoftDelete)) != null)
                        {
                            entry.State = EntityState.Unchanged; //先将状态设为Unchanged，再设置IsDeleted列为true，避免更新全部字段
                            //entry.State = EntityState.Modified;
                            entry.CurrentValues[column_IsDeleted] = true;
                        }
                        if (typeEntity.GetInterface(nameof(IDeletionAudited)) != null)
                        {
                            Session.VerifyLoggedin();
                            entry.CurrentValues[nameof(IDeletionAudited.DeleterUserId)] = Session.UserId;
                            entry.CurrentValues[nameof(IDeletionAudited.DeletionTime)] = DateTime.Now;
                        }
                        break;
                }
            }
        }

    }

    public class TodoContextFactory : IDesignTimeDbContextFactory<TodoContext>
    {
        public TodoContext CreateDbContext(string[] args)
        {
            const string connectionString = "server=10.1.8.40;userid=root;pwd=Passw0rd;port=3306;database=XactTodo;";
            var optionsBuilder = new DbContextOptionsBuilder<TodoContext>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            //optionsBuilder.UseMySql("server=10.1.17.201;userid=root;pwd=123456;port=3306;database=XactTodo;");

            return new TodoContext(optionsBuilder.Options, null);
        }
    }
}
