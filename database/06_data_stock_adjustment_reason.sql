--
-- PostgreSQL database dump
--

SET statement_timeout = 0;
SET lock_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET client_min_messages = warning;

SET search_path = public, pg_catalog;

--
-- Data for Name: m_reason_penyesuaian_stok; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY m_adjustment_reason(stock_adjustment_reason_id, reason) FROM stdin;
83bbf9aa-0d45-7041-e53b-d6c9073a49c0	Lost (Lost item)
e4ef2a27-6600-365f-1e07-2963d55cc4bf	Correction (Correction due to input error)
1c23364b-e65d-62ef-4180-b2f3f7f560c1	Damaged (Damaged item)
b1ad1bca-b590-2231-06a3-8c3cc5445eaf	Beginning Balance (Initial stock balance)
f227318c-72dc-5284-6b00-58005d511043	Stock Opname (Stock opname or physical inventory count)
f9b35798-6725-244f-fec0-fdee38c5ad44	Moved from warehouse to display area
7aa5ecff-4ed3-2e57-ead3-a493d822ab96	Other
7092373e-91df-4c3a-b98a-1fc8d09d91e8	Used for personal use
\.


--
-- PostgreSQL database dump complete
--

