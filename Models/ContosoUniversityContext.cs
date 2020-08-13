using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace webapiProject.Models {

    public partial class ContosoUniversityContext : DbContext {

        public ContosoUniversityContext() {
        }

        public ContosoUniversityContext(DbContextOptions<ContosoUniversityContext> options)
            : base(options) {
        }

        public virtual DbSet<Course> Course { get; set; }
        public virtual DbSet<CourseInstructor> CourseInstructor { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<Enrollment> Enrollment { get; set; }
        public virtual DbSet<OfficeAssignment> OfficeAssignment { get; set; }
        public virtual DbSet<Person> Person { get; set; }
        public virtual DbSet<VwCourseStudentCount> VwCourseStudentCount { get; set; }
        public virtual DbSet<VwCourseStudents> VwCourseStudents { get; set; }
        public virtual DbSet<VwDepartmentCourseCount> VwDepartmentCourseCount { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (!optionsBuilder.IsConfigured) {
                optionsBuilder.UseSqlServer("Server=(localdb)\\.;Initial Catalog=ContosoUniversity;integrated security=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Course>(entity => {
                entity.HasQueryFilter(e => e.IsDeleted == false);

                entity.HasIndex(e => e.DepartmentId)
                    .HasName("IX_DepartmentID");

                entity.Property(e => e.CourseId).HasColumnName("CourseID");

                entity.Property(e => e.DepartmentId)
                    .HasColumnName("DepartmentID")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Title).HasMaxLength(50);

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Course)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_dbo.Course_dbo.Department_DepartmentID");
            });

            modelBuilder.Entity<CourseInstructor>(entity => {
                entity.HasKey(e => new { e.CourseId, e.InstructorId })
                    .HasName("PK_dbo.CourseInstructor");

                entity.HasIndex(e => e.CourseId)
                    .HasName("IX_CourseID");

                entity.HasIndex(e => e.InstructorId)
                    .HasName("IX_InstructorID");

                entity.Property(e => e.CourseId).HasColumnName("CourseID");

                entity.Property(e => e.InstructorId).HasColumnName("InstructorID");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.CourseInstructor)
                    .HasForeignKey(d => d.CourseId)
                    .HasConstraintName("FK_dbo.CourseInstructor_dbo.Course_CourseID");

                entity.HasOne(d => d.Instructor)
                    .WithMany(p => p.CourseInstructor)
                    .HasForeignKey(d => d.InstructorId)
                    .HasConstraintName("FK_dbo.CourseInstructor_dbo.Instructor_InstructorID");
            });

            modelBuilder.Entity<Department>(entity => {
                entity.HasQueryFilter(e => e.IsDeleted == false);

                entity.HasIndex(e => e.InstructorId)
                    .HasName("IX_InstructorID");

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.Property(e => e.Budget).HasColumnType("money");

                entity.Property(e => e.InstructorId).HasColumnName("InstructorID");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.HasOne(d => d.Instructor)
                    .WithMany(p => p.Department)
                    .HasForeignKey(d => d.InstructorId)
                    .HasConstraintName("FK_dbo.Department_dbo.Instructor_InstructorID");
            });

            modelBuilder.Entity<Enrollment>(entity => {
                entity.HasIndex(e => e.CourseId)
                    .HasName("IX_CourseID");

                entity.HasIndex(e => e.StudentId)
                    .HasName("IX_StudentID");

                entity.Property(e => e.EnrollmentId).HasColumnName("EnrollmentID");

                entity.Property(e => e.CourseId).HasColumnName("CourseID");

                entity.Property(e => e.StudentId).HasColumnName("StudentID");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Enrollment)
                    .HasForeignKey(d => d.CourseId)
                    .HasConstraintName("FK_dbo.Enrollment_dbo.Course_CourseID");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Enrollment)
                    .HasForeignKey(d => d.StudentId)
                    .HasConstraintName("FK_dbo.Enrollment_dbo.Person_StudentID");
            });

            modelBuilder.Entity<OfficeAssignment>(entity => {
                entity.HasKey(e => e.InstructorId)
                    .HasName("PK_dbo.OfficeAssignment");

                entity.HasIndex(e => e.InstructorId)
                    .HasName("IX_InstructorID");

                entity.Property(e => e.InstructorId)
                    .HasColumnName("InstructorID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Location).HasMaxLength(50);

                entity.HasOne(d => d.Instructor)
                    .WithOne(p => p.OfficeAssignment)
                    .HasForeignKey<OfficeAssignment>(d => d.InstructorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.OfficeAssignment_dbo.Instructor_InstructorID");
            });

            modelBuilder.Entity<Person>(entity => {
                entity.HasQueryFilter(e => e.IsDeleted == false);

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Discriminator)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasDefaultValueSql("('Instructor')");

                entity.Property(e => e.EnrollmentDate).HasColumnType("datetime");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.HireDate).HasColumnType("datetime");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<VwCourseStudentCount>(entity => {
                entity.HasNoKey();

                entity.ToView("vwCourseStudentCount");

                entity.Property(e => e.CourseId).HasColumnName("CourseID");

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Title).HasMaxLength(50);
            });

            modelBuilder.Entity<VwCourseStudents>(entity => {
                entity.HasNoKey();

                entity.ToView("vwCourseStudents");

                entity.Property(e => e.CourseId).HasColumnName("CourseID");

                entity.Property(e => e.CourseTitle).HasMaxLength(50);

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.Property(e => e.DepartmentName).HasMaxLength(50);

                entity.Property(e => e.StudentId).HasColumnName("StudentID");

                entity.Property(e => e.StudentName).HasMaxLength(101);
            });

            modelBuilder.Entity<VwDepartmentCourseCount>(entity => {
                entity.HasNoKey();

                entity.ToView("vwDepartmentCourseCount");

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken()) {
            DateTime now = DateTime.Now;
            var entityEntrys = ChangeTracker.Entries();
            foreach (EntityEntry entry in entityEntrys) {

                #region Department entity

                if (typeof(Department).IsInstanceOfType(entry.Entity)) {
                    var department = (Department)entry.Entity;
                    if (entry.State == EntityState.Added) {
                        var outDepartment = this.Department.FromSqlRaw(
                            $"EXEC [dbo].[Department_Insert] " +
                            $"'{ department.Name }', '{ department.Budget }', " +
                            $"'{ department.StartDate }', '{ department.InstructorId }'")
                        .IgnoreQueryFilters().AsEnumerable().FirstOrDefault();

                        department.DepartmentId = outDepartment.DepartmentId;
                        department.RowVersion = outDepartment.RowVersion;
                    } else if (entry.State == EntityState.Modified) {
                        var sql =
                            $"EXEC [dbo].[Department_Update] " +
                            $"@DepartmentId, @Name, @Budget, " +
                            $"@StartDate, @InstructorID, " +
                            $"@RowVersion_Original, @DateModified";
                        var parameters = new List<SqlParameter> {
                            new SqlParameter("@DepartmentId", department.DepartmentId),
                            new SqlParameter("@Name", department.Name),
                            new SqlParameter("@Budget", department.Budget),
                            new SqlParameter("@StartDate", department.StartDate),
                            new SqlParameter("@InstructorId", department.InstructorId),
                            new SqlParameter("@RowVersion_Original", department.RowVersion),
                            new SqlParameter("@DateModified", now)
                        };
                        Database.ExecuteSqlRaw(sql, parameters);
                    } else if (entry.State == EntityState.Deleted) {
                        var sql = $"EXEC [dbo].[Department_Delete] @DepartmentId, @RowVersion_Original";
                        var parameters = new List<SqlParameter> {
                            new SqlParameter("@DepartmentId", department.DepartmentId),
                            new SqlParameter("@RowVersion_Original", department.RowVersion)
                        };
                        Database.ExecuteSqlRaw(sql, parameters);
                    }
                    entry.State = EntityState.Detached;
                }

                #endregion Department entity

                #region Course entity

                if (typeof(Course).IsInstanceOfType(entry.Entity)) {
                    var course = (Course)entry.Entity;
                    if (entry.State == EntityState.Modified) {
                        course.DateModified = now;
                    } else if (entry.State == EntityState.Deleted) {
                        course.IsDeleted = true;
                        entry.State = EntityState.Modified;
                    }
                }

                #endregion Course entity

                #region Person entity

                if (typeof(Person).IsInstanceOfType(entry.Entity)) {
                    var person = (Person)entry.Entity;
                    if (entry.State == EntityState.Modified) {
                        person.DateModified = now;
                    } else if (entry.State == EntityState.Deleted) {
                        person.IsDeleted = true;
                        entry.State = EntityState.Modified;
                    }
                }

                #endregion Person entity
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}