--
-- PostgreSQL database dump
--


-- Dumped from database version 18.4
-- Dumped by pg_dump version 18.4

-- Started on 2026-07-08 14:24:43

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 4 (class 2615 OID 2200)
-- Name: public; Type: SCHEMA; Schema: -; Owner: pg_database_owner
--

CREATE SCHEMA public;


ALTER SCHEMA public OWNER TO pg_database_owner;

--
-- TOC entry 4932 (class 0 OID 0)
-- Dependencies: 4
-- Name: SCHEMA public; Type: COMMENT; Schema: -; Owner: pg_database_owner
--

COMMENT ON SCHEMA public IS 'standard public schema';


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 224 (class 1259 OID 16409)
-- Name: author; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.author (
    author_id integer NOT NULL,
    first_name character varying(100) NOT NULL,
    last_name character varying(100) NOT NULL
);


ALTER TABLE public.author OWNER TO postgres;

--
-- TOC entry 223 (class 1259 OID 16408)
-- Name: author_author_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.author ALTER COLUMN author_id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.author_author_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 222 (class 1259 OID 16396)
-- Name: publication; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.publication (
    publication_id integer NOT NULL,
    title character varying(255) NOT NULL,
    publication_year smallint,
    isbn character varying(20),
    publisher_id integer
);


ALTER TABLE public.publication OWNER TO postgres;

--
-- TOC entry 221 (class 1259 OID 16395)
-- Name: publication_publication_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.publication ALTER COLUMN publication_id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.publication_publication_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 225 (class 1259 OID 16417)
-- Name: publicationauthor; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.publicationauthor (
    publication_id integer NOT NULL,
    author_id integer NOT NULL
);


ALTER TABLE public.publicationauthor OWNER TO postgres;

--
-- TOC entry 220 (class 1259 OID 16388)
-- Name: publisher; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.publisher (
    publisher_id integer NOT NULL,
    name character varying(255) NOT NULL
);


ALTER TABLE public.publisher OWNER TO postgres;

--
-- TOC entry 219 (class 1259 OID 16387)
-- Name: publisher_publisher_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.publisher ALTER COLUMN publisher_id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.publisher_publisher_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 4774 (class 2606 OID 16416)
-- Name: author author_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.author
    ADD CONSTRAINT author_pkey PRIMARY KEY (author_id);


--
-- TOC entry 4772 (class 2606 OID 16402)
-- Name: publication publication_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.publication
    ADD CONSTRAINT publication_pkey PRIMARY KEY (publication_id);


--
-- TOC entry 4776 (class 2606 OID 16423)
-- Name: publicationauthor publicationauthor_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.publicationauthor
    ADD CONSTRAINT publicationauthor_pkey PRIMARY KEY (publication_id, author_id);


--
-- TOC entry 4770 (class 2606 OID 16394)
-- Name: publisher publisher_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.publisher
    ADD CONSTRAINT publisher_pkey PRIMARY KEY (publisher_id);


--
-- TOC entry 4778 (class 2606 OID 16429)
-- Name: publicationauthor fk_pa_author; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.publicationauthor
    ADD CONSTRAINT fk_pa_author FOREIGN KEY (author_id) REFERENCES public.author(author_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4779 (class 2606 OID 16424)
-- Name: publicationauthor fk_pa_publication; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.publicationauthor
    ADD CONSTRAINT fk_pa_publication FOREIGN KEY (publication_id) REFERENCES public.publication(publication_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4777 (class 2606 OID 16403)
-- Name: publication fk_publication_publisher; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.publication
    ADD CONSTRAINT fk_publication_publisher FOREIGN KEY (publisher_id) REFERENCES public.publisher(publisher_id) ON UPDATE CASCADE ON DELETE RESTRICT;


-- Completed on 2026-07-08 14:24:43

--
-- PostgreSQL database dump complete
--


