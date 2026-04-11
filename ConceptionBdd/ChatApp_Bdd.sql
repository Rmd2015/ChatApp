-- ======================================================
-- Script PostgreSQL - Version avec CHATUSER
-- Date : 10/04/2026
-- ======================================================

-- Suppression propre des tables
DROP TABLE IF EXISTS USER_RECIVE CASCADE;
DROP TABLE IF EXISTS MSG_CONTIENT CASCADE;
DROP TABLE IF EXISTS MSG_APPARTIENT CASCADE;
DROP TABLE IF EXISTS TOKENS CASCADE;
DROP TABLE IF EXISTS CHATROOMMEMBER CASCADE;
DROP TABLE IF EXISTS CHATROOM CASCADE;
DROP TABLE IF EXISTS MESSAGE CASCADE;
DROP TABLE IF EXISTS ATTACHMENT CASCADE;
DROP TABLE IF EXISTS CHATUSER CASCADE;

-- ======================================================
-- Table : CHATUSER
-- ======================================================
CREATE TABLE CHATUSER (
    IDUSER          BIGSERIAL PRIMARY KEY,
    USERNAME        VARCHAR(50) UNIQUE NOT NULL,
    PASSWORD        VARCHAR(60) NOT NULL,
    ISCONNECT       BOOLEAN DEFAULT FALSE,
    CREATEDAT       TIMESTAMPTZ DEFAULT NOW()
);

-- ======================================================
-- Table : ATTACHMENT
-- ======================================================
CREATE TABLE ATTACHMENT (
    IDATTACHMENT    BIGSERIAL PRIMARY KEY,
    ATTACHMENTSIZE  VARCHAR(30),
    ATTACHMENTPATH  VARCHAR(150) NOT NULL,
    CREATEDAT       TIMESTAMPTZ DEFAULT NOW()
);

-- ======================================================
-- Table : CHATROOM
-- ======================================================
CREATE TABLE CHATROOM (
    IDCHATROOM      BIGSERIAL PRIMARY KEY,
    IDUSER          BIGINT NOT NULL,                    -- créateur
    CHATROOMLABEL     VARCHAR(30),
    ROOMCREATEDAT   TIMESTAMPTZ DEFAULT NOW(),
    
    CONSTRAINT FK_CHATROOM_CREATOR 
        FOREIGN KEY (IDUSER) REFERENCES CHATUSER(IDUSER) ON DELETE RESTRICT
);

-- ======================================================
-- Table : CHATROOMMEMBER
-- ======================================================
CREATE TABLE CHATROOMMEMBER (
    IDCHATROOM      BIGINT NOT NULL,
    IDUSER          BIGINT NOT NULL,
    USERADDAT       TIMESTAMPTZ DEFAULT NOW(),
    
    CONSTRAINT PK_CHATROOMMEMBER 
        PRIMARY KEY (IDCHATROOM, IDUSER),
        
    CONSTRAINT FK_CHATROOMMEMBER_CHATROOM 
        FOREIGN KEY (IDCHATROOM) REFERENCES CHATROOM(IDCHATROOM) ON DELETE CASCADE,
        
    CONSTRAINT FK_CHATROOMMEMBER_USER 
        FOREIGN KEY (IDUSER) REFERENCES CHATUSER(IDUSER) ON DELETE CASCADE
);

-- ======================================================
-- Table : MESSAGE
-- ======================================================
CREATE TABLE MESSAGE (
    IDMSG           BIGSERIAL PRIMARY KEY,
    IDUSER          BIGINT NOT NULL,                    -- expéditeur
    CONTENT         TEXT,
    CREATEDAT       TIMESTAMPTZ DEFAULT NOW(),
    
    CONSTRAINT FK_MESSAGE_SENDER 
        FOREIGN KEY (IDUSER) REFERENCES CHATUSER(IDUSER) ON DELETE RESTRICT
);

-- ======================================================
-- Table : MSG_APPARTIENT
-- ======================================================
CREATE TABLE MSG_APPARTIENT (
    IDMSG           BIGINT NOT NULL,
    IDCHATROOM      BIGINT NOT NULL,
    MSGSENDCHATROOMAT TIMESTAMPTZ DEFAULT NOW(),
    
    CONSTRAINT PK_MSG_APPARTIENT 
        PRIMARY KEY (IDMSG, IDCHATROOM),
        
    CONSTRAINT FK_MSG_APPARTIENT_MESSAGE 
        FOREIGN KEY (IDMSG) REFERENCES MESSAGE(IDMSG) ON DELETE CASCADE,
        
    CONSTRAINT FK_MSG_APPARTIENT_CHATROOM 
        FOREIGN KEY (IDCHATROOM) REFERENCES CHATROOM(IDCHATROOM) ON DELETE CASCADE
);

-- ======================================================
-- Table : MSG_CONTIENT
-- ======================================================
CREATE TABLE MSG_CONTIENT (
    IDMSG           BIGINT NOT NULL,
    IDATTACHMENT    BIGINT NOT NULL,
    
    CONSTRAINT PK_MSG_CONTIENT 
        PRIMARY KEY (IDMSG, IDATTACHMENT),
        
    CONSTRAINT FK_MSG_CONTIENT_MESSAGE 
        FOREIGN KEY (IDMSG) REFERENCES MESSAGE(IDMSG) ON DELETE CASCADE,
        
    CONSTRAINT FK_MSG_CONTIENT_ATTACHMENT 
        FOREIGN KEY (IDATTACHMENT) REFERENCES ATTACHMENT(IDATTACHMENT) ON DELETE CASCADE
);

-- ======================================================
-- Table : TOKENS
-- ======================================================
CREATE TABLE TOKENS (
    IDTOKEN         BIGSERIAL PRIMARY KEY,
    IDUSER          BIGINT NOT NULL,
    TOKEN           VARCHAR(300) NOT NULL UNIQUE,
    EXPIRESAT       TIMESTAMPTZ NOT NULL,
    CREATEDAT       TIMESTAMPTZ DEFAULT NOW(),
    ISVALID         BOOLEAN DEFAULT TRUE,
    
    CONSTRAINT FK_TOKENS_USER 
        FOREIGN KEY (IDUSER) REFERENCES CHATUSER(IDUSER) ON DELETE CASCADE
);

-- ======================================================
-- Table : USER_RECIVE
-- ======================================================
CREATE TABLE USER_RECIVE (
    IDUSER          BIGINT NOT NULL,
    IDMSG           BIGINT NOT NULL,
    RECIVEDAT       TIMESTAMPTZ DEFAULT NOW(),
    MSGVUE          BOOLEAN DEFAULT FALSE,
    VUEAT           TIMESTAMPTZ,
    
    CONSTRAINT PK_USER_RECIVE 
        PRIMARY KEY (IDUSER, IDMSG),
        
    CONSTRAINT FK_USER_RECIVE_USER 
        FOREIGN KEY (IDUSER) REFERENCES CHATUSER(IDUSER) ON DELETE CASCADE,
        
    CONSTRAINT FK_USER_RECIVE_MESSAGE 
        FOREIGN KEY (IDMSG) REFERENCES MESSAGE(IDMSG) ON DELETE CASCADE
);
CREATE TABLE "Tokens" (
    "Id"            BIGSERIAL PRIMARY KEY,
    "Token"         TEXT NOT NULL,
    "ExpiresAt"     TIMESTAMPTZ NOT NULL,
    "CreatedAt"     TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "IsValid"       BOOLEAN NOT NULL DEFAULT true
);

-- Index pour les performances
CREATE INDEX idx_chatroommember_user ON CHATROOMMEMBER(IDUSER);
CREATE INDEX idx_msg_appartient_chatroom ON MSG_APPARTIENT(IDCHATROOM);
CREATE INDEX idx_user_recive_user ON USER_RECIVE(IDUSER);
CREATE INDEX idx_message_createdat ON MESSAGE(CREATEDAT DESC);
CREATE INDEX idx_tokens_user ON TOKENS(IDUSER);
