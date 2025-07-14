
    drop table LOGIN_TOKENS_SHOUQ cascade constraints;

    drop table MERCHANT_BRANCHES_SHOUQ cascade constraints;

    drop table MERCHANT_GROUPS_SHOUQ cascade constraints;

    drop table MERCHANTS_SHOUQ cascade constraints;

    drop table USERS_SHOUQ cascade constraints;

    drop sequence hibernate_sequence;

    drop sequence SEQ_MERCHANT_BRANCHES_SHOUQ;

    drop sequence SEQ_MERCHANT_GROUPS_SHOUQ;

    drop sequence SEQ_MERCHANTS_SHOUQ;

    drop sequence SEQ_USERS_SHOUQ;

    create table LOGIN_TOKENS_SHOUQ (
        Id NUMBER(10,0) not null,
       UserId NUMBER(10,0) not null,
       Token VARCHAR2(4000) not null,
       ExpiryDate TIMESTAMP(7) not null,
       CreatedAt TIMESTAMP(7) default SYSDATE  not null,
       primary key (Id)
    );

    create table MERCHANT_BRANCHES_SHOUQ (
        Id NUMBER(10,0) not null,
       BRANCH_NAME_AR VARCHAR2(255),
       BRANCH_NAME_EN VARCHAR2(255),
       CITY_ID NUMBER(10,0),
       GOVERNATE_ID NUMBER(10,0),
       AL_HAT VARCHAR2(255),
       ADDRESS VARCHAR2(255),
       REGION VARCHAR2(255),
       FAX VARCHAR2(255),
       WEBSITE VARCHAR2(255),
       PHONE VARCHAR2(255),
       MOBILE VARCHAR2(255),
       GPS VARCHAR2(255),
       STATUS NUMBER(10,0),
       MAIN_BRANCH VARCHAR2(255),
       DELETED_AT TIMESTAMP(7),
       CREATED_AT TIMESTAMP(7),
       UPDATED_AT TIMESTAMP(7),
       MERCHANT_ID NUMBER(10,0) not null,
       USER_ID NUMBER(10,0),
       primary key (Id)
    );

    create table MERCHANT_GROUPS_SHOUQ (
        Id NUMBER(10,0) not null,
       NAME_AR VARCHAR2(255),
       NAME_EN VARCHAR2(255),
       CREATED_AT TIMESTAMP(7),
       UPDATED_AT TIMESTAMP(7),
       DELETED_AT TIMESTAMP(7),
       primary key (Id)
    );

    create table MERCHANTS_SHOUQ (
        Id NUMBER(10,0) not null,
       NAME_AR VARCHAR2(255),
       NAME_EN VARCHAR2(255),
       BUSINESS_TYPE NUMBER(10,0),
       STATUS NUMBER(10,0),
       DELETED_AT TIMESTAMP(7),
       CREATED_AT TIMESTAMP(7),
       UPDATED_AT TIMESTAMP(7),
       MANAGER_NAME VARCHAR2(255),
       MERCHANT_GROUP_ID NUMBER(10,0) not null,
       primary key (Id)
    );

    create table USERS_SHOUQ (
        Id NUMBER(10,0) not null,
       USERNAME VARCHAR2(255),
       EMAIL VARCHAR2(255),
       PASSWORD_HASH VARCHAR2(255),
       CREATED_AT TIMESTAMP(7),
       DELETED_AT TIMESTAMP(7),
       primary key (Id)
    );

    alter table MERCHANT_BRANCHES_SHOUQ 
        add constraint FK_4A684F88 
        foreign key (MERCHANT_ID) 
        references MERCHANTS_SHOUQ;

    alter table MERCHANT_BRANCHES_SHOUQ 
        add constraint FK_49A870C3 
        foreign key (USER_ID) 
        references USERS_SHOUQ;

    alter table MERCHANTS_SHOUQ 
        add constraint FK_A9DCAB92 
        foreign key (MERCHANT_GROUP_ID) 
        references MERCHANT_GROUPS_SHOUQ;

    create sequence hibernate_sequence;

    create sequence SEQ_MERCHANT_BRANCHES_SHOUQ;

    create sequence SEQ_MERCHANT_GROUPS_SHOUQ;

    create sequence SEQ_MERCHANTS_SHOUQ;

    create sequence SEQ_USERS_SHOUQ;
