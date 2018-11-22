using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace VodManageSystem.Models.DataModels
{
    /// <summary>
    /// Ktv system DBC ontext.
    /// </summary>
    public partial class KtvSystemDBContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:VodManageSystem.Models.DataModels.KtvSystemDBContext"/> class.
        /// for being added to services
        /// </summary>
        /// <param name="options">Options.</param>
        public KtvSystemDBContext(DbContextOptions<KtvSystemDBContext> options) : base(options)
        {
            // empty constructor
        }

        public virtual DbSet<BehaWd> BehaWd { get; set; }
        public virtual DbSet<Computer> Computer { get; set; }
        public virtual DbSet<Kkk2> Kkk2 { get; set; }
        public virtual DbSet<Language> Language { get; set; }
        public virtual DbSet<Phonetic> Phonetic { get; set; }
        public virtual DbSet<Singarea> Singarea { get; set; }
        public virtual DbSet<Singer> Singer { get; set; }
        public virtual DbSet<Song> Song { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Playerscore> Playerscore { get; set; }

        /*
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("server=localhost;userid=chaolee;password=86637971;database=KtvSystemDB;");
            }
        }
        */

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BehaWd>(entity =>
            {
                entity.HasKey(e => e.ChinW);

                entity.ToTable("beha_wd");

                entity.Property(e => e.ChinW)
                    .HasColumnName("chin_w")
                    .HasMaxLength(2);

                entity.Property(e => e.NumFw).HasColumnName("num_fw");

                entity.Property(e => e.NumPw)
                    .IsRequired()
                    .HasColumnName("num_pw")
                    .HasMaxLength(1);
            });

            modelBuilder.Entity<Computer>(entity =>
            {
                entity.ToTable("computer");

                entity.HasIndex(e => e.ComputerId)
                    .HasName("computer_id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.ComputerId)
                    .HasColumnName("computer_id")
                    .HasMaxLength(5);

                entity.Property(e => e.BranchId)
                    .IsRequired()
                    .HasColumnName("branch_id")
                    .HasMaxLength(5);

                entity.Property(e => e.RoomNo)
                    .IsRequired()
                    .HasColumnName("room_no")
                    .HasMaxLength(3);

                entity.Property(e => e.SongNo)
                    .IsRequired()
                    .HasColumnName("song_no")
                    .HasMaxLength(6);
            });

            modelBuilder.Entity<Kkk2>(entity =>
            {
                entity.HasKey(e => e.RNo);

                entity.ToTable("kkk2");

                entity.Property(e => e.RNo)
                    .HasColumnName("r_no")
                    .HasMaxLength(3);

                entity.Property(e => e.BadTf)
                    .HasColumnName("bad_tf")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.BegTime)
                    .IsRequired()
                    .HasColumnName("beg_time")
                    .HasMaxLength(4);

                entity.Property(e => e.CastAa)
                    .IsRequired()
                    .HasColumnName("cast_aa")
                    .HasMaxLength(1);

                entity.Property(e => e.CastStr1)
                    .IsRequired()
                    .HasColumnName("cast_str1")
                    .HasMaxLength(40);

                entity.Property(e => e.CastStr2)
                    .IsRequired()
                    .HasColumnName("cast_str2")
                    .HasMaxLength(40);

                entity.Property(e => e.CastStr3)
                    .IsRequired()
                    .HasColumnName("cast_str3")
                    .HasMaxLength(40);

                entity.Property(e => e.ChckTf)
                    .HasColumnName("chck_tf")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.ComputerId)
                    .IsRequired()
                    .HasColumnName("computer_id")
                    .HasMaxLength(4);

                entity.Property(e => e.DisServ)
                    .IsRequired()
                    .HasColumnName("dis_serv")
                    .HasMaxLength(1);

                entity.Property(e => e.Downecho)
                    .HasColumnName("downecho")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.Downkey)
                    .HasColumnName("downkey")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.Downmic)
                    .HasColumnName("downmic")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.Downmusc)
                    .HasColumnName("downmusc")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.Dp8Tf)
                    .HasColumnName("dp8_tf")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.EjTf)
                    .HasColumnName("ej_tf")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.EnServ)
                    .IsRequired()
                    .HasColumnName("en_serv")
                    .HasMaxLength(1);

                entity.Property(e => e.Ffward)
                    .HasColumnName("ffward")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.FormNo)
                    .IsRequired()
                    .HasColumnName("form_no")
                    .HasMaxLength(12);

                entity.Property(e => e.Illegal)
                    .HasColumnName("illegal")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.Mobile)
                    .IsRequired()
                    .HasColumnName("mobile")
                    .HasMaxLength(6);

                entity.Property(e => e.Mult)
                    .IsRequired()
                    .HasColumnName("mult")
                    .HasMaxLength(1);

                entity.Property(e => e.Mute)
                    .HasColumnName("mute")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.NetNo)
                    .IsRequired()
                    .HasColumnName("net_no")
                    .HasMaxLength(12);

                entity.Property(e => e.OffTime).HasColumnName("off_time");

                entity.Property(e => e.OpenTf)
                    .HasColumnName("open_tf")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.Pause)
                    .HasColumnName("pause")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.PlSong)
                    .IsRequired()
                    .HasColumnName("pl_song")
                    .HasMaxLength(6);

                entity.Property(e => e.PwId)
                    .IsRequired()
                    .HasColumnName("pw_id")
                    .HasMaxLength(2);

                entity.Property(e => e.PwctTf)
                    .HasColumnName("pwct_tf")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.Repeat)
                    .IsRequired()
                    .HasColumnName("repeat")
                    .HasMaxLength(1);

                entity.Property(e => e.Rmno)
                    .IsRequired()
                    .HasColumnName("rmno")
                    .HasMaxLength(3);

                entity.Property(e => e.RomId)
                    .IsRequired()
                    .HasColumnName("rom_id")
                    .HasMaxLength(3);

                entity.Property(e => e.ServYn)
                    .IsRequired()
                    .HasColumnName("serv_yn")
                    .HasMaxLength(1);

                entity.Property(e => e.Sn1)
                    .IsRequired()
                    .HasColumnName("sn1")
                    .HasMaxLength(6);

                entity.Property(e => e.Sp11)
                    .HasColumnName("sp11")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.Sp12)
                    .HasColumnName("sp12")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.SpNo)
                    .IsRequired()
                    .HasColumnName("sp_no")
                    .HasMaxLength(6);

                entity.Property(e => e.Stdkey)
                    .HasColumnName("stdkey")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.Tm)
                    .IsRequired()
                    .HasColumnName("tm")
                    .HasMaxLength(1);

                entity.Property(e => e.UpdTf)
                    .HasColumnName("upd_tf")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.Upecho)
                    .HasColumnName("upecho")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.Upkey)
                    .HasColumnName("upkey")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.Upmic)
                    .HasColumnName("upmic")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.Upmusc)
                    .HasColumnName("upmusc")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.VvvId).HasColumnName("vvv_id");
            });

            modelBuilder.Entity<Language>(entity =>
            {
                entity.ToTable("language");

                entity.HasIndex(e => e.LangNo)
                    .HasName("lang_no_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.LangEn)
                    .IsRequired()
                    .HasColumnName("lang_en")
                    .HasMaxLength(20);

                entity.Property(e => e.LangNa)
                    .IsRequired()
                    .HasColumnName("lang_na")
                    .HasMaxLength(8);

                entity.Property(e => e.LangNo)
                    .IsRequired()
                    .HasColumnName("lang_no")
                    .HasMaxLength(2);
            });

            modelBuilder.Entity<Phonetic>(entity =>
            {
                entity.HasKey(e => e.PhnE);

                entity.ToTable("phonetic");

                entity.Property(e => e.PhnE)
                    .HasColumnName("phn_e")
                    .HasMaxLength(10);

                entity.Property(e => e.ChWord)
                    .IsRequired()
                    .HasColumnName("ch_word")
                    .HasMaxLength(254);

                entity.Property(e => e.Pinyin)
                    .IsRequired()
                    .HasColumnName("pinyin")
                    .HasMaxLength(16);

                entity.Property(e => e.Pinyin2)
                    .IsRequired()
                    .HasColumnName("pinyin_2")
                    .HasMaxLength(16);
            });

            modelBuilder.Entity<Singarea>(entity =>
            {
                entity.ToTable("singarea");

                entity.HasIndex(e => e.AreaNo)
                    .HasName("area_no_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AreaEn)
                    .IsRequired()
                    .HasColumnName("area_en")
                    .HasMaxLength(28);

                entity.Property(e => e.AreaNa)
                    .IsRequired()
                    .HasColumnName("area_na")
                    .HasMaxLength(14);

                entity.Property(e => e.AreaNo)
                    .IsRequired()
                    .HasColumnName("area_no")
                    .HasMaxLength(2);
            });

            modelBuilder.Entity<Singer>(entity =>
            {
                entity.ToTable("singer");

                entity.HasIndex(e => e.AreaId)
                    .HasName("SingerAreaFKey_idx");

                entity.HasIndex(e => e.SingNo)
                    .HasName("sing_no_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AreaId)
                    .HasColumnName("area_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Chor)
                    .IsRequired()
                    .HasColumnName("chor")
                    .HasMaxLength(1);

                entity.Property(e => e.Hot)
                    .IsRequired()
                    .HasColumnName("hot")
                    .HasMaxLength(1);

                entity.Property(e => e.NumFw).HasColumnName("num_fw");

                entity.Property(e => e.NumPw)
                    .IsRequired()
                    .HasColumnName("num_pw")
                    .HasMaxLength(1);

                entity.Property(e => e.PicFile)
                    .IsRequired()
                    .HasColumnName("pic_file")
                    .HasMaxLength(5);

                entity.Property(e => e.Sex)
                    .IsRequired()
                    .HasColumnName("sex")
                    .HasMaxLength(1);

                entity.Property(e => e.SingNa)
                    .IsRequired()
                    .HasColumnName("sing_na")
                    .HasMaxLength(30);

                entity.Property(e => e.SingNo)
                    .IsRequired()
                    .HasColumnName("sing_no")
                    .HasMaxLength(5);

                entity.HasOne(d => d.Singarea)
                    .WithMany(p => p.Singers)
                    .HasForeignKey(d => d.AreaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SingerArea");
            });

            modelBuilder.Entity<Song>(entity =>
            {
                entity.ToTable("song");

                // removed on 2017-12-04
                // entity.HasKey(e => e.SongNo); 

                entity.HasIndex(e => e.SongNo)
                    .HasName("song_no_UNIQUE")
                    .IsUnique();

                // added on 2017-12-04
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.HasIndex(e => e.LanguageId)
                    .HasName("SongLangFKey_idx");

                entity.HasIndex(e => e.Singer1Id)
                    .HasName("SongSinger1FKey_idx");

                entity.HasIndex(e => e.Singer2Id)
                    .HasName("SongSinger2FKey_idx");

                entity.Property(e => e.SongNo)
                    .IsRequired()
                    .HasColumnName("song_no")
                    .HasMaxLength(6);

                entity.Property(e => e.Chor)
                    .IsRequired()
                    .HasColumnName("chor")
                    .HasMaxLength(1);

                entity.Property(e => e.InDate)
                    .HasColumnName("in_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.LanguageId)
                    .IsRequired()
                    .HasColumnName("language_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.MMpeg)
                    .IsRequired()
                    .HasColumnName("m_mpeg")
                    .HasMaxLength(2);

                entity.Property(e => e.NMpeg)
                    .IsRequired()
                    .HasColumnName("n_mpeg")
                    .HasMaxLength(2);

                entity.Property(e => e.NumFw)
                    .IsRequired()
                    .HasColumnName("num_fw")
                    .HasColumnType("int(2)");

                entity.Property(e => e.NumPw)
                    .IsRequired()
                    .HasColumnName("num_pw")
                    .HasMaxLength(1);

                entity.Property(e => e.OrdNo)
                    .HasColumnName("ord_no")
                    .HasColumnType("int(6)");

                entity.Property(e => e.OrdOldN)
                    .HasColumnName("ord_old_n")
                    .HasColumnType("int(6)");

                entity.Property(e => e.OrderNum)
                    .HasColumnName("order_num")
                    .HasColumnType("int(6)");

                entity.Property(e => e.Pathname)
                    .IsRequired()
                    .HasColumnName("pathname")
                    .HasMaxLength(6);

                entity.Property(e => e.SNumWord)
                    .IsRequired()
                    .HasColumnName("s_num_word")
                    .HasColumnType("int(2)");
                      
                entity.Property(e => e.SeleTf)
                    .HasColumnName("sele_tf")
                    .HasMaxLength(1);

                entity.Property(e => e.Singer1Id)
                    .IsRequired()
                    .HasColumnName("singer1_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Singer2Id)
                    .IsRequired()
                    .HasColumnName("singer2_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SongNa)
                    .IsRequired()
                    .HasColumnName("song_na")
                    .HasMaxLength(45);

                entity.Property(e => e.VodNo)
                    .IsRequired()
                    .HasColumnName("vod_no")
                    .HasMaxLength(6);

                entity.Property(e => e.VodYn)
                    .IsRequired()
                    .HasColumnName("vod_yn")
                    .HasMaxLength(1);

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.Songs)
                    .HasForeignKey(d => d.LanguageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SongLangFKey");

                entity.HasOne(d => d.Singer1)
                    .WithMany(p => p.SongSinger1s)
                    .HasForeignKey(d => d.Singer1Id)
                    .HasConstraintName("SongSinger1FKey");

                entity.HasOne(d => d.Singer2)
                    .WithMany(p => p.SongSinger2s)
                    .HasForeignKey(d => d.Singer2Id)
                    .HasConstraintName("SongSinger2FKey");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.UserId)
                    .HasName("userId_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UserEmail)
                    .HasColumnName("userEmail")
                    .HasMaxLength(50);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("userId")
                    .HasMaxLength(8);

                entity.Property(e => e.UserName)
                    .HasColumnName("userName")
                    .HasMaxLength(30);

                entity.Property(e => e.UserPassword)
                    .IsRequired()
                    .HasColumnName("userPassword")
                    .HasMaxLength(16);

                entity.Property(e => e.UserState)
                    .HasColumnName("userState")
                    .HasColumnType("tinytext");
            });

            modelBuilder.Entity<Playerscore>(entity =>
            {
                entity.ToTable("playerscore");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();
                
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PlayerName)
                    .HasColumnName("playername")
                    .HasMaxLength(45);

                entity.Property(e => e.Score)
                    .HasColumnName("score")
                    .HasColumnType("int(12)");

                entity.Property(e => e.GameId)
                    .HasColumnName("game_id")
                    .HasColumnType("int(11)");

            });
        }
    }
}
