-- =====================================================
-- SCHEMA COMPLET POUR SUPABASE - APPLICATION CHAT
-- =====================================================

-- Supprimer les tables existantes (dans l'ordre correct pour éviter les erreurs de dépendance)
DROP TABLE IF EXISTS MSG_CONTIENT;
DROP TABLE IF EXISTS MSG_APPARTIENT;
DROP TABLE IF EXISTS USER_RECIVE;
DROP TABLE IF EXISTS CHATROOMMEMBER;
DROP TABLE IF EXISTS ATTACHMENT;
DROP TABLE IF EXISTS MESSAGE;
DROP TABLE IF EXISTS CHATROOM;
DROP TABLE IF EXISTS chatuser;

-- =====================================================
-- Table : USER
-- =====================================================
CREATE TABLE chatuser (
    IDUSER          BIGSERIAL PRIMARY KEY,
    USERNAME        VARCHAR(50) UNIQUE NOT NULL,
    PASSWORD        VARCHAR(60) NOT NULL,
    ISCONNECT       BOOLEAN DEFAULT false,
    CREATEDAT       TIMESTAMPTZ DEFAULT NOW()
);

-- =====================================================
-- Table : CHATROOM
-- =====================================================
CREATE TABLE CHATROOM (
    IDCHATROOM      BIGSERIAL PRIMARY KEY,
    IDUSER          BIGINT NOT NULL,           -- Créateur du salon
    ROOMCREATEDAT   TIMESTAMPTZ DEFAULT NOW(),

    CONSTRAINT FK_CHATROOM_USER_CREA_USER 
        FOREIGN KEY (IDUSER) REFERENCES chatuser(IDUSER) 
        ON DELETE CASCADE ON UPDATE CASCADE
);

-- =====================================================
-- Table : CHATROOMMEMBER
-- =====================================================
CREATE TABLE CHATROOMMEMBER (
    IDCHATROOM      BIGINT NOT NULL,
    IDUSER          BIGINT NOT NULL,
    USERADDAT       TIMESTAMPTZ DEFAULT NOW(),

    CONSTRAINT PK_CHATROOMMEMBER 
        PRIMARY KEY (IDCHATROOM, IDUSER),

    CONSTRAINT FK_CHATROOM_CHATROOMM_USER 
        FOREIGN KEY (IDUSER) REFERENCES chatuser(IDUSER) 
        ON DELETE CASCADE ON UPDATE CASCADE,

    CONSTRAINT FK_CHATROOM_CHATROOMM_CHATROOM 
        FOREIGN KEY (IDCHATROOM) REFERENCES CHATROOM(IDCHATROOM) 
        ON DELETE CASCADE ON UPDATE CASCADE
);

-- =====================================================
-- Table : MESSAGE
-- =====================================================
CREATE TABLE MESSAGE (
    IDMSG           BIGSERIAL PRIMARY KEY,
    IDUSER          BIGINT NOT NULL,           -- Expéditeur du message
    CONTENT         TEXT,
    CREATEDAT       TIMESTAMPTZ DEFAULT NOW(),

    CONSTRAINT FK_MESSAGE_USER_SEND_USER 
        FOREIGN KEY (IDUSER) REFERENCES chatuser(IDUSER) 
        ON DELETE CASCADE ON UPDATE CASCADE
);

-- =====================================================
-- Table : ATTACHMENT
-- =====================================================
CREATE TABLE ATTACHMENT (
    IDATTACHMENT    BIGSERIAL PRIMARY KEY,
    ATTACHMENTSIZE  VARCHAR(30),
    ATTACHMENTPATH  VARCHAR(150)
);

-- =====================================================
-- Table : MSG_APPARTIENT (Lien entre Message et ChatRoom)
-- =====================================================
CREATE TABLE MSG_APPARTIENT (
    IDMSG           BIGINT NOT NULL,
    IDCHATROOM      BIGINT NOT NULL,
    MSGSENDCHATROOMAT TIMESTAMPTZ DEFAULT NOW(),

    CONSTRAINT PK_MSG_APPARTIENT 
        PRIMARY KEY (IDMSG, IDCHATROOM),

    CONSTRAINT FK_MSG_APPA_MSG_APPAR_CHATROOM 
        FOREIGN KEY (IDCHATROOM) REFERENCES CHATROOM(IDCHATROOM) 
        ON DELETE CASCADE ON UPDATE CASCADE,

    CONSTRAINT FK_MSG_APPA_MSG_APPAR_MESSAGE 
        FOREIGN KEY (IDMSG) REFERENCES MESSAGE(IDMSG) 
        ON DELETE CASCADE ON UPDATE CASCADE
);

-- =====================================================
-- Table : MSG_CONTIENT (Lien entre Message et Attachment)
-- =====================================================
CREATE TABLE MSG_CONTIENT (
    IDMSG           BIGINT NOT NULL,
    IDATTACHMENT    BIGINT NOT NULL,

    CONSTRAINT PK_MSG_CONTIENT 
        PRIMARY KEY (IDMSG, IDATTACHMENT),

    CONSTRAINT FK_MSG_CONT_MSG_CONTI_ATTACHME 
        FOREIGN KEY (IDATTACHMENT) REFERENCES ATTACHMENT(IDATTACHMENT) 
        ON DELETE CASCADE ON UPDATE CASCADE,

    CONSTRAINT FK_MSG_CONT_MSG_CONTI_MESSAGE 
        FOREIGN KEY (IDMSG) REFERENCES MESSAGE(IDMSG) 
        ON DELETE CASCADE ON UPDATE CASCADE
);

-- =====================================================
-- Table : USER_RECIVE (Statut de lecture des messages)
-- =====================================================
CREATE TABLE USER_RECIVE (
    IDUSER          BIGINT NOT NULL,
    IDMSG           BIGINT NOT NULL,
    RECIVEDAT       TIMESTAMPTZ DEFAULT NOW(),
    MSGVUE          BOOLEAN DEFAULT false,
    VUEAT           TIMESTAMPTZ,

    CONSTRAINT PK_USER_RECIVE 
        PRIMARY KEY (IDUSER, IDMSG),

    CONSTRAINT FK_USER_REC_USER_RECI_MESSAGE 
        FOREIGN KEY (IDMSG) REFERENCES MESSAGE(IDMSG) 
        ON DELETE CASCADE ON UPDATE CASCADE,

    CONSTRAINT FK_USER_REC_USER_RECI_USER 
        FOREIGN KEY (IDUSER) REFERENCES chatuser(IDUSER) 
        ON DELETE CASCADE ON UPDATE CASCADE
);
CREATE TABLE "Tokens" (
    "Id"            BIGSERIAL PRIMARY KEY,
    "Token"         TEXT NOT NULL,
    "ExpiresAt"     TIMESTAMPTZ NOT NULL,
    "CreatedAt"     TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "IsValid"       BOOLEAN NOT NULL DEFAULT true
);

-- Index pour améliorer les performances (très recommandé)
CREATE INDEX idx_tokens_token ON "Tokens"("Token");
CREATE INDEX idx_tokens_expiresat ON "Tokens"("ExpiresAt");
CREATE INDEX idx_tokens_isvalid ON "Tokens"("IsValid");
-- =====================================================
-- Index pour améliorer les performances
-- =====================================================
CREATE INDEX idx_message_chatroom ON MSG_APPARTIENT(IDCHATROOM);
CREATE INDEX idx_message_sender ON MESSAGE(IDUSER);
CREATE INDEX idx_user_recive_msg ON USER_RECIVE(IDMSG);
CREATE INDEX idx_chatroommember_user ON CHATROOMMEMBER(IDUSER);
