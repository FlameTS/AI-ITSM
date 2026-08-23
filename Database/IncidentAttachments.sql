-- M2-2.3 — Incident Attachments Database Extension
-- AI-ITSM
-- Adds only the attachment metadata table required by M2.
-- Existing tables are not modified.

IF OBJECT_ID(N'dbo.IncidentAttachments', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[IncidentAttachments]
    (
        [AttachmentId] [int] IDENTITY(1,1) NOT NULL,
        [IncidentId] [int] NOT NULL,
        [FileName] [varchar](255) NOT NULL,
        [StoredFileName] [varchar](255) NOT NULL,
        [ContentType] [varchar](100) NOT NULL,
        [FileSize] [bigint] NOT NULL,
        [UploadedAt] [datetime] NOT NULL
            CONSTRAINT [DF_IncidentAttachments_UploadedAt]
            DEFAULT (GETDATE()),

        CONSTRAINT [PK_IncidentAttachments]
            PRIMARY KEY CLUSTERED ([AttachmentId] ASC),

        CONSTRAINT [FK_IncidentAttachments_Incidents]
            FOREIGN KEY ([IncidentId])
            REFERENCES [dbo].[Incidents] ([IncidentId])
    );

    CREATE INDEX [IX_IncidentAttachments_IncidentId]
        ON [dbo].[IncidentAttachments] ([IncidentId]);
END;
GO

-- Verification
SELECT
    OBJECT_ID(N'dbo.IncidentAttachments', N'U') AS TableId;

SELECT
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'dbo'
  AND TABLE_NAME = 'IncidentAttachments'
ORDER BY ORDINAL_POSITION;
