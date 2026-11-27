-- SQL Script to Create GyanPath Database
-- CT050-3-2-WAPP Web Applications Assignment Project: GYANPATH

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = N'GyanPathDB')
BEGIN
    CREATE DATABASE GyanPathDB;
END
GO

USE GyanPathDB;
GO
