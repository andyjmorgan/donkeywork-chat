﻿// <auto-generated />
using System;
using System.Collections.Generic;
using DonkeyWork.Chat.Common.Providers;
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
    [Migration("20250416172006_ApiKeys")]
    partial class ApiKeys
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("ApiPersistence")
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

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable((string)null);

                    b.UseTpcMappingStrategy();
                });

            modelBuilder.Entity("DonkeyWork.Chat.Persistence.Entity.ApiKey.ApiKeyEntity", b =>
                {
                    b.HasBaseType("DonkeyWork.Chat.Persistence.Entity.Base.BaseUserEntity");

                    b.Property<string>("ApiKey")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("Description")
                        .HasMaxLength(1024)
                        .HasColumnType("character varying(1024)");

                    b.Property<bool>("Enabled")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.HasIndex("ApiKey")
                        .IsUnique();

                    b.ToTable("ApiKeys", "ApiPersistence");
                });

            modelBuilder.Entity("DonkeyWork.Chat.Persistence.Entity.Conversation.ConversationEntity", b =>
                {
                    b.HasBaseType("DonkeyWork.Chat.Persistence.Entity.Base.BaseUserEntity");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.ToTable("Conversations", "ApiPersistence");
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

                    b.ToTable("ConversationMessages", "ApiPersistence");
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

                    b.ToTable("ToolCalls", "ApiPersistence");
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

                    b.ToTable("Prompts", "ApiPersistence");
                });

            modelBuilder.Entity("DonkeyWork.Chat.Persistence.Entity.Provider.UserTokenEntity", b =>
                {
                    b.HasBaseType("DonkeyWork.Chat.Persistence.Entity.Base.BaseUserEntity");

                    b.Property<Dictionary<UserProviderDataKeyType, string>>("Data")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<DateTimeOffset?>("ExpiresAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("ProviderType")
                        .HasColumnType("integer");

                    b.Property<List<string>>("Scopes")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.ToTable("UserTokens", "ApiPersistence");
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
