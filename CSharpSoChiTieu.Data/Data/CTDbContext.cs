using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CSharpSoChiTieu.Data
{
    public class CTDbContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public CTDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
            //AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        /// <summary>   
        /// Connection DB SQL Server
        /// </summary>
        /// <param name="options"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(Configuration.GetConnectionString("WebSoChiTieuDbManagement_SqlServer"));
        }

        #region DBSet
        /// <summary>
        /// Test Table
        /// </summary>
        public DbSet<ct_User> ct_Users { get; set; } // User dang nhap
        public DbSet<ct_IncomeExpense> ct_IncomeExpense { get; set; }
        public DbSet<ct_IncomeExpenseCategory> ct_IncomeExpenseCategories { get; set; }
        #endregion DBSet


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Cấu hình các bản
            modelBuilder.Entity<ct_User>(entity => {
                // Đảm bảo UserName là duy nhất
                entity.HasIndex(e => e.UserName).IsUnique();

                // Cấu hình giới hạn cột
                entity.Property(e => e.UserName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Phone).HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Role).HasMaxLength(50);
            });

            modelBuilder.Entity<ct_IncomeExpense>(entity =>
            {
                // Cấu hình cột Name, Amount, Description, ...
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.Description).HasMaxLength(255);

                // Cấu hình Status (Thu hoặc Chi)
                entity.Property(e => e.Type).IsRequired();
            });

            modelBuilder.Entity<ct_IncomeExpenseCategory>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            });
            #endregion đặt giới hạn cho colum

            #region Nối bản

            // Foreign key {N ct_IncomeExpense - 1 ct_CategoryIncomeExpense}
            modelBuilder.Entity<ct_IncomeExpense>()
                .HasOne(c => c.Category)
                .WithMany(c => c.ct_IncomeExpense)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);



            #endregion Nối bản
        }
    }
}