using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GSBTest.Models;

public partial class GsbtestContext : DbContext
{
    public GsbtestContext()
    {
    }

    public GsbtestContext(DbContextOptions<GsbtestContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblAccount> TblAccounts { get; set; }

    public virtual DbSet<TblAdmin> TblAdmins { get; set; }

    public virtual DbSet<TblBasvuru> TblBasvurus { get; set; }

    public virtual DbSet<TblLog> TblLogs { get; set; }


    public virtual DbSet<TblRef> TblRefs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=GSBTest;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblAccount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tblAccount");

            entity.ToTable("tbl_Account");

            entity.Property(e => e.KayitDurumu).HasColumnName("Kayit_Durumu");
            entity.Property(e => e.KullaniciAdi)
                .HasMaxLength(50)
                .HasColumnName("Kullanici_Adi");
            entity.Property(e => e.KullaniciEmail)
                .HasMaxLength(50)
                .HasColumnName("Kullanici_Email");
            entity.Property(e => e.KullaniciSifre)
                .HasMaxLength(50)
                .HasColumnName("Kullanici_Sifre");
            entity.Property(e => e.SillinmeDurumu).HasColumnName("Sillinme_Durumu");
            entity.Property(e => e.YetkiBasvuruIslemleri).HasColumnName("Yetki_Basvuru_Islemleri");
            entity.Property(e => e.YetkiKullaniciIslemleri).HasColumnName("Yetki_Kullanici_Islemleri");
            entity.Property(e => e.YetkiReferansIslemleri).HasColumnName("Yetki_Referans_Islemleri");
        });

        modelBuilder.Entity<TblAdmin>(entity =>
        {
            entity.ToTable("tbl_Admin");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AdminAdi).HasMaxLength(50);
            entity.Property(e => e.AdminEmail).HasMaxLength(50);
            entity.Property(e => e.AdminSifre).HasMaxLength(50);
            entity.Property(e => e.AdminSoyadi).HasMaxLength(50);
        });

        modelBuilder.Entity<TblBasvuru>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_basvuru");

            entity.ToTable("tbl_Basvuru");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AciklamaTarihi)
                .HasColumnType("datetime")
                .HasColumnName("ACIKLAMA_TARIHI");
            entity.Property(e => e.BasvuranBirim)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BASVURAN_BIRIM");
            entity.Property(e => e.BasvuruDonemi)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BASVURU_DONEMI");
            entity.Property(e => e.BasvuruDurumu)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BASVURU_DURUMU");
            entity.Property(e => e.BasvuruTarih)
                .HasColumnType("datetime")
                .HasColumnName("BASVURU_TARIH");
            entity.Property(e => e.BasvuruYapilanProje)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BASVURU_YAPILAN_PROJE");
            entity.Property(e => e.BasvuruYapilanTur)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BASVURU_YAPILAN_TUR");
            entity.Property(e => e.HibeTutari).HasColumnName("HIBE_TUTARI");
            entity.Property(e => e.KatilimciTuru)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("KATILIMCI_TURU");
            entity.Property(e => e.ProjeAdi)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PROJE_ADI");
        });

        modelBuilder.Entity<TblLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__tbl_Log__5E54864818706208");

            entity.ToTable("tbl_Log");

            entity.Property(e => e.DateTime).HasColumnType("datetime");
            entity.Property(e => e.Hata).HasMaxLength(255);
            entity.Property(e => e.LogType).HasMaxLength(50);
            entity.Property(e => e.MethodName).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.TblLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__tbl_Log__UserId__02FC7413");
        });

     

        modelBuilder.Entity<TblRef>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tbl_ref");

            entity.ToTable("tbl_Ref");

            entity.Property(e => e.Alttip)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SilinmeDurumu).HasColumnName("Silinme_Durumu");
            entity.Property(e => e.Tip)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
