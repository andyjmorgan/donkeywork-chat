﻿// <auto-generated />
using System;
using DonkeyWork.Chat.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DonkeyWork.Chat.Persistence.Migrations
{
    [DbContext(typeof(ApiPersistenceContext))]
    [Migration("20250319111506_AddPrompts")]
    partial class AddPrompts
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DonkeyWork.Chat.Persistence.Entity.Base.BaseUserEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(34)
                        .HasColumnType("character varying(34)");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("BaseUserEntity");

                    b.HasDiscriminator().HasValue("BaseUserEntity");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("DonkeyWork.Chat.Persistence.Entity.Conversation.ConversationEntity", b =>
                {
                    b.HasBaseType("DonkeyWork.Chat.Persistence.Entity.Base.BaseUserEntity");

                    b.Property<int>("MessageCount")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.HasDiscriminator().HasValue("ConversationEntity");
                });

            modelBuilder.Entity("DonkeyWork.Chat.Persistence.Entity.Conversation.ConversationMessageEntity", b =>
                {
                    b.HasBaseType("DonkeyWork.Chat.Persistence.Entity.Base.BaseUserEntity");

                    b.Property<Guid>("ConversationId")
                        .HasColumnType("uuid");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("MessageOwner")
                        .HasColumnType("integer");

                    b.Property<Guid>("MessagePairId")
                        .HasColumnType("uuid");

                    b.HasIndex("ConversationId");

                    b.HasIndex("MessagePairId");

                    b.HasDiscriminator().HasValue("ConversationMessageEntity");
                });

            modelBuilder.Entity("DonkeyWork.Chat.Persistence.Entity.Conversation.ToolCallEntity", b =>
                {
                    b.HasBaseType("DonkeyWork.Chat.Persistence.Entity.Base.BaseUserEntity");

                    b.Property<Guid>("ConversationId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("MessagePairId")
                        .HasColumnType("uuid");

                    b.Property<string>("Request")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Response")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ToolName")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.HasIndex("ConversationId");

                    b.HasIndex("MessagePairId");

                    b.ToTable("BaseUserEntity", t =>
                        {
                            t.Property("ConversationId")
                                .HasColumnName("ToolCallEntity_ConversationId");

                            t.Property("MessagePairId")
                                .HasColumnName("ToolCallEntity_MessagePairId");
                        });

                    b.HasDiscriminator().HasValue("ToolCallEntity");
                });

            modelBuilder.Entity("DonkeyWork.Chat.Persistence.Entity.Prompt.PromptEntity", b =>
                {
                    b.HasBaseType("DonkeyWork.Chat.Persistence.Entity.Base.BaseUserEntity");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<int>("UsageCount")
                        .HasColumnType("integer");

                    b.ToTable("BaseUserEntity", t =>
                        {
                            t.Property("Title")
                                .HasColumnName("PromptEntity_Title");
                        });

                    b.HasDiscriminator().HasValue("PromptEntity");
                });

            modelBuilder.Entity("DonkeyWork.Chat.Persistence.Entity.Conversation.ConversationMessageEntity", b =>
                {
                    b.HasOne("DonkeyWork.Chat.Persistence.Entity.Conversation.ConversationEntity", "Conversation")
                        .WithMany("MessageEntities")
                        .HasForeignKey("ConversationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Conversation");
                });

            modelBuilder.Entity("DonkeyWork.Chat.Persistence.Entity.Conversation.ToolCallEntity", b =>
                {
                    b.HasOne("DonkeyWork.Chat.Persistence.Entity.Conversation.ConversationEntity", "Conversation")
                        .WithMany("ToolCallEntities")
                        .HasForeignKey("ConversationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Conversation");
                });

            modelBuilder.Entity("DonkeyWork.Chat.Persistence.Entity.Conversation.ConversationEntity", b =>
                {
                    b.Navigation("MessageEntities");

                    b.Navigation("ToolCallEntities");
                });
#pragma warning restore 612, 618
        }
    }
}
