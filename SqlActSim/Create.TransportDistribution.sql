﻿CREATE TABLE [dbo].[TransportDistribution]
(
    [Name]                        NVARCHAR (MAX) NULL,
    [Description]                 NVARCHAR (MAX) NULL,
    [Cost]                 NVARCHAR (MAX) NULL,
    [Cost unit]                    NVARCHAR (MAX) NULL,
    [Fuel quantity]                 NVARCHAR (MAX) NULL,
    [Fuel quantity unit]                    NVARCHAR (MAX) NULL,
    [Fuel type]                    NVARCHAR (MAX) NULL,
    [Goods quantity (mass)]              NVARCHAR (MAX) NULL,
    [Goods quantity (mass) unit]      NVARCHAR (MAX) NULL,
    [Distance] NVARCHAR (MAX) NULL,
    [Distance unit] NVARCHAR (MAX) NULL,
    [Data Quality Type]           NVARCHAR (MAX) NULL,
    [Facility]                    NVARCHAR (MAX) NULL,
    [Organizational Unit]         NVARCHAR (MAX) NULL,
    [Transport mode]               NVARCHAR (MAX) NULL,
    [Transaction Date]            NVARCHAR (MAX) NULL,
    [Consumption Start Date]      NVARCHAR (MAX) NULL,
    [Consumption End Date]        NVARCHAR (MAX) NULL,
    [Evidence]                    NVARCHAR (MAX) NULL,
    [Origin Correlation ID]       NVARCHAR (MAX) NULL,
    [Industrial process type]             NVARCHAR (MAX) NULL,
    [Quantity]                NVARCHAR (MAX) NULL,
    [Quantity unit]                NVARCHAR (MAX) NULL,
    [Spend type]                NVARCHAR (MAX) NULL,
    [Value chain partner]               NVARCHAR (MAX) NULL
)
