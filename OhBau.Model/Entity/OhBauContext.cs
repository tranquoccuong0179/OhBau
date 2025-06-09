using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OhBau.Model.Enum;
namespace OhBau.Model.Entity;

public partial class OhBauContext : DbContext
{
    public OhBauContext()
    {
    }

    public OhBauContext(DbContextOptions<OhBauContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Chapter> Chapters { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<DoctorSlot> DoctorSlots { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Fetus> Fetus { get; set; }

    public virtual DbSet<FetusDetail> FetusDetails { get; set; }

    public virtual DbSet<Major> Majors { get; set; }

    public virtual DbSet<MotherHealthRecord> MotherHealthRecords { get; set; }

    public virtual DbSet<MyCourse> MyCourses { get; set; }

    public virtual DbSet<Parent> Parents { get; set; }

    public virtual DbSet<ParentRelation> ParentRelations { get; set; }

    public virtual DbSet<Slot> Slots { get; set; }

    public virtual DbSet<FeedBacks> FeedBacks { get; set; }

    public virtual DbSet<Cart> Cart { get; set; }

    public virtual DbSet<CartItems> CartItems { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<Comments> Comments { get; set; }

    public virtual DbSet<CourseRating> CourseRating { get; set; }   

    public virtual DbSet<FavoriteCourses> FavoriteCourses { get; set; }
    public virtual DbSet<Topic> Topics { get; set;}
    public virtual DbSet<LikeBlog> LikeBlog { get; set; }
    public virtual DbSet<Product> Products {  get; set; }
    public virtual DbSet<ProductCategory> ProductCategory { get; set; }


    public static string GetConnectionString(string connectionStringName)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        string connectionString = config.GetConnectionString(connectionStringName);
        return connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(GetConnectionString("DefautDB")).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("Account");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.DeleteAt).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Topic>().ToTable("Topics");

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.ToTable("Booking");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.DeleteAt).HasColumnType("datetime");
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.DotorSlot).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.DotorSlotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_DoctorSlot_1");

            entity.HasOne(d => d.Parent).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_Parent");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Chapter>(entity =>
        {
            entity.ToTable("Chapter");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.DeleteAt).HasColumnType("datetime");
            entity.Property(e => e.ImageUrl).IsUnicode(false);
            entity.Property(e => e.Title).HasMaxLength(50);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");
            entity.Property(e => e.VideoUrl).IsUnicode(false);

            entity.HasOne(d => d.Topic).WithMany(p => p.Chapters)
                .HasForeignKey(d => d.TopicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Chapter_Course");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Course");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.DeleteAt).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.Category).WithMany(p => p.Courses)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Course_Category");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.ToTable("Doctor");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.Avatar).HasMaxLength(255);
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.DeleteAt).HasColumnType("datetime");
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.Gender)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.Account).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Doctor_Account");

            entity.HasOne(d => d.Major).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.MajorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Doctor_Major");
        });

        modelBuilder.Entity<DoctorSlot>(entity =>
        {
            entity.ToTable("DoctorSlot");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.DeleteAt).HasColumnType("datetime");
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.Doctor).WithMany(p => p.DoctorSlots)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DoctorSlot_Doctor");

            entity.HasOne(d => d.Slot).WithMany(p => p.DoctorSlots)
                .HasForeignKey(d => d.SlotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DoctorSlot_Slot_1");
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.ToTable("Favorite");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreateAt).HasColumnType("datetime");

            entity.HasOne(d => d.Account).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Favorite_Account");

            entity.HasOne(d => d.Course).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Favorite_Course_1");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.BookingId).HasColumnName("BookingID");
            entity.Property(e => e.Content).HasMaxLength(255);

            entity.HasOne(d => d.Booking).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_Feedbacks_Booking");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK_Feedbacks_Doctor");
        });

        modelBuilder.Entity<Fetus>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.DeleteAt).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<FetusDetail>(entity =>
        {
            entity.ToTable("FetusDetail");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Ac).HasColumnName("AC");
            entity.Property(e => e.Weight).HasColumnName("Weight");
            entity.Property(e => e.Height).HasColumnName("Height");
            entity.Property(e => e.Bpd).HasColumnName("BPD");
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.Crl).HasColumnName("CRL");
            entity.Property(e => e.DeleteAt).HasColumnType("datetime");
            entity.Property(e => e.Fl).HasColumnName("FL");
            entity.Property(e => e.Gsd).HasColumnName("GSD");
            entity.Property(e => e.Hc).HasColumnName("HC");
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.Fetus).WithMany(p => p.FetusDetails)
                .HasForeignKey(d => d.FetusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FetusDetail_Fetus");
        });

        modelBuilder.Entity<Major>(entity =>
        {
            entity.ToTable("Major");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.DeleteAt).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<MotherHealthRecord>(entity =>
        {
            entity.ToTable("MotherHealthRecord");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.DeleteAt).HasColumnType("datetime");
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.Parent).WithMany(p => p.MotherHealthRecords)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MotherHealthRecord_Parent");
        });

        modelBuilder.Entity<MyCourse>(entity =>
        {
            entity.ToTable("MyCourse");

            entity.Property(e => e.CreateAt).HasColumnType("datetime");

            entity.HasOne(d => d.Account).WithMany(p => p.MyCourses)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MyCourse_Account");

            entity.HasOne(d => d.Course).WithMany(p => p.MyCourses)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MyCourse_Course_1");
        });

        modelBuilder.Entity<Parent>(entity =>
        {
            entity.ToTable("Parent");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.DeleteAt).HasColumnType("datetime");
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.FullName).HasMaxLength(50);
            //entity.Property(e => e.CCCD).HasMaxLength(50);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.Account).WithMany(p => p.Parents)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_Parent_Account");
        });

        modelBuilder.Entity<ParentRelation>(entity =>
        {
            entity.ToTable("ParentRelation");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.DeleteAt).HasColumnType("datetime");
            entity.Property(e => e.RelationType).HasMaxLength(50);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.Account).WithMany(p => p.ParentRelations)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_ParentRelation_Account");

            entity.HasOne(d => d.Fetus).WithMany(p => p.ParentRelations)
                .HasForeignKey(d => d.FetusId)
                .HasConstraintName("FK_ParentRelation_Fetus_2");

            entity.HasOne(d => d.Parent).WithMany(p => p.ParentRelations)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_ParentRelation_Parent_1");
        });

        modelBuilder.Entity<Slot>(entity =>
        {
            entity.ToTable("Slot");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.DeleteAt).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Order>().Property(o => o.PaymentStatus).HasConversion<string>();
        modelBuilder.Entity<Blog>().Property(b => b.Status).HasConversion<string>();

        modelBuilder.Entity<FavoriteCourses>().HasKey(c => new { c.AccountId, c.CourseId });
        modelBuilder.Entity<MyCourse>().HasKey(c => new {c.AccountId, c.CourseId});
        modelBuilder.Entity<Transaction>().Property(b => b.Provider).HasConversion<string>();
        modelBuilder.Entity<Transaction>().Property(c => c.Status).HasConversion<string>();
        modelBuilder.Entity<Transaction>().Property(c => c.Type).HasConversion<string>();

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(p => p.ProductCategory)
                  .WithMany(pc => pc.Products)
                  .HasForeignKey("CategoryId")
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_Products_ProductCategory");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.ToTable("ProductCategory");
            entity.Property(e => e.Id).ValueGeneratedNever();
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

}
