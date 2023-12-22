--
-- Name: m_regency2; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_regency2 (
    regency_id character(4) NOT NULL,
    province_id character(2),
    name_regency t_address_panjang
);


ALTER TABLE m_regency2 OWNER TO postgres;

--
-- Name: m_card; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_card (
    card_id t_guid NOT NULL,
    card_name t_name,
    is_debit t_bool
);


ALTER TABLE m_card OWNER TO postgres;

--
-- Name: m_employee; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_employee (
    employee_id t_guid NOT NULL,
    job_titles_id t_guid,
    employee_name t_name,
    address t_address,
    phone t_phone,
    basic_salary t_price,
    is_active t_bool,
    description t_description,
    payment_type integer DEFAULT 1,
    overtime_salary t_price DEFAULT 0,
    total_loan t_price,
    total_loan_payment t_price
);


ALTER TABLE m_employee OWNER TO postgres;

--
-- Name: m_subdistrict; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_subdistrict (
    subdistrict_id character(7) NOT NULL,
    regency_id character(4),
    name_subdistrict t_address_panjang
);


ALTER TABLE m_subdistrict OWNER TO postgres;

--
-- Name: m_invoice_label; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_invoice_label (
    invoice_label_id t_guid NOT NULL,
    description t_description,
    order_number integer,
    is_active t_bool
);


ALTER TABLE m_invoice_label OWNER TO postgres;

--
-- Name: m_menu; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_menu (
    menu_id t_guid NOT NULL,
    name_menu t_name,
    menu_title t_description,
    parent_id t_guid,
    order_number integer,
    is_active t_bool,
    name_form t_description,
    is_enabled t_bool
);


ALTER TABLE m_menu OWNER TO postgres;

--
-- Name: COLUMN m_menu.parent_id; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN m_menu.parent_id IS 'Diisi dengan menu_id';


--
-- Name: m_user; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_user (
    user_id t_guid NOT NULL,
    role_id t_guid,
    name_user t_name,
    user_password t_password,
    is_active t_bool,
    status_user integer DEFAULT 2,
    email t_description
);


ALTER TABLE m_user OWNER TO postgres;

--
-- Name: COLUMN m_user.status_user; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN m_user.status_user IS '1 = Kasir
2 = Server
3 = Kasir dan Server';


--
-- Name: m_invoice_prefix; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_invoice_prefix (
    prefix_invoice_id integer DEFAULT 1 NOT NULL,
    invoice_prefix character varying(3),
    description t_description
);


ALTER TABLE m_invoice_prefix OWNER TO postgres;

--
-- Name: m_product; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_product (
    product_id t_guid NOT NULL,
    product_name t_name_panjang,
    unit t_unit,
    stok t_quantity,
    purchase_price t_price,
    iselling_price t_price,
    product_code t_product_code,
    category_id t_guid,
    minimal_stok t_quantity DEFAULT 0,
    warehouse_stock t_quantity,
    minimal_stock_warehouse t_quantity,
    discount t_quantity,
    profit_percentage t_quantity,
    is_active t_bool,
    last_update timestamp(0) without time zone DEFAULT now()
);


ALTER TABLE m_product OWNER TO postgres;

--
-- Name: m_product_product_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE m_product_product_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE m_product_product_id_seq OWNER TO postgres;

--
-- Name: m_profile; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_profile (
    profile_id t_guid NOT NULL,
    name_profile t_description,
    address t_address,
    city t_address,
    phone t_phone,
    email t_description,
    website t_description,
    register_id t_guid,
    is_register boolean DEFAULT false,
    hash t_description
);


ALTER TABLE m_profile OWNER TO postgres;

--
-- Name: m_province; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_province (
    province_id integer NOT NULL,
    name_province t_description
);


ALTER TABLE m_province OWNER TO postgres;

--
-- Name: m_province2; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_province2 (
    province_id character(2) NOT NULL,
    name_province t_address_panjang
);


ALTER TABLE m_province2 OWNER TO postgres;

--
-- Name: m_role; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_role (
    role_id t_guid NOT NULL,
    name_role t_name,
    is_active t_bool
);


ALTER TABLE m_role OWNER TO postgres;

--
-- Name: m_role_privilege; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_role_privilege (
    role_id t_guid NOT NULL,
    menu_id t_guid NOT NULL,
    grant_id integer NOT NULL,
    is_grant t_bool
);


ALTER TABLE m_role_privilege OWNER TO postgres;

--
-- Name: COLUMN m_role_privilege.grant_id; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN m_role_privilege.grant_id IS 'Tambah, Perbaiki, Hapus, Dll';


--
-- Name: m_application_setting; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_application_setting (
    application_setting_id t_guid NOT NULL,
    is_update_iselling_price_master_produk t_bool,
    is_negative_stock_allowed_for_products t_bool,
    is_focus_on_inputting_quantity_column t_bool,
    is_tampilkan_description_tambahan_item_jual t_bool,
    description_tambahan_item_jual t_description
);


ALTER TABLE m_application_setting OWNER TO postgres;

--
-- Name: m_shift; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_shift (
    shift_id t_guid NOT NULL,
    name_shift t_description,
    start_time timestamp without time zone,
    end_time timestamp without time zone,
    is_active t_bool
);


ALTER TABLE m_shift OWNER TO postgres;

--
-- Name: m_supplier; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_supplier (
    supplier_id t_guid NOT NULL,
    name_supplier t_name,
    address t_address,
    contact t_name,
    phone t_phone,
    total_debt t_price,
    total_debt_payment t_price
);


ALTER TABLE m_supplier OWNER TO postgres;

--
-- Name: t_purchase_product; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_purchase_product (
    beli_product_id t_guid NOT NULL,
    user_id t_guid,
    supplier_id t_guid,
    retur_beli_product_id t_guid,
    nota t_nota,
    date date,
    date_tempo date,
    tax t_price,
    discount t_price,
    total_invoice t_price,
    total_payment t_price,
    description t_description,
    system_date timestamp without time zone DEFAULT now()
);


ALTER TABLE t_purchase_product OWNER TO postgres;

--
-- Name: t_purchase_product_beli_product_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE t_purchase_product_beli_product_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_purchase_product_beli_product_id_seq OWNER TO postgres;

--
-- Name: t_employee_salary; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_employee_salary (
    gaji_employee_id t_guid NOT NULL,
    employee_id t_guid,
    user_id t_guid,
    month integer,
    year integer,
    attendance integer,
    absence integer,
    basic_salary t_price,
    overtime t_price,
    bonus t_price,
    deductions t_price,
    system_date timestamp without time zone DEFAULT now(),
    time integer DEFAULT 0,
    other t_price DEFAULT 0,
    description t_description,
    days_worked integer DEFAULT 0,
    allowance t_price DEFAULT 0,
    loan t_price DEFAULT 0,
    date date,
    nota t_nota
);


ALTER TABLE t_employee_salary OWNER TO postgres;

--
-- Name: t_employee_salary_gaji_employee_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE t_employee_salary_gaji_employee_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_employee_salary_gaji_employee_id_seq OWNER TO postgres;

--
-- Name: t_purchase_order_item; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_purchase_order_item (
    item_beli_product_id t_guid NOT NULL,
    beli_product_id t_guid,
    user_id t_guid,
    product_id t_guid,
    price t_price,
    quantity t_quantity,
    discount t_quantity,
    system_date timestamp without time zone DEFAULT now(),
    return_quantity t_quantity
);


ALTER TABLE t_purchase_order_item OWNER TO postgres;

--
-- Name: t_sales_order_item; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_sales_order_item (
    item_sale_id t_guid NOT NULL,
    sale_id t_guid,
    user_id t_guid,
    product_id t_guid,
    purchase_price t_price,
    iselling_price t_price,
    quantity t_quantity,
    discount t_quantity,
    system_date timestamp without time zone DEFAULT now(),
    return_quantity t_quantity,
    description t_description
);


ALTER TABLE t_sales_order_item OWNER TO postgres;

--
-- Name: t_debt_payment_item; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_debt_payment_item (
    item_pembayaran_hutang_product_id t_guid NOT NULL,
    pembayaran_hutang_product_id t_guid,
    beli_product_id t_guid,
    amount t_price,
    description t_description,
    system_date timestamp without time zone DEFAULT now()
);


ALTER TABLE t_debt_payment_item OWNER TO postgres;

--
-- Name: t_credit_payment_item; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_credit_payment_item(
    pay_sale_item_id t_guid NOT NULL,
    pay_sale_id t_guid,
    sale_id t_guid,
    amount t_price,
    description t_description,
    system_date timestamp without time zone DEFAULT now()
);


ALTER TABLE t_credit_payment_item OWNER TO postgres;

--
-- Name: t_expense_item; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_expense_item(
    expense_item_id t_guid NOT NULL,
    expense_id t_guid,
    user_id t_guid,
    quantity t_quantity,
    price t_price,
    system_date timestamp without time zone DEFAULT now(),
    expense_type_id t_guid
);


ALTER TABLE t_expense_item OWNER TO postgres;

--
-- Name: t_purchase_return_item; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_purchase_return_item(
    item_retur_beli_product_id t_guid NOT NULL,
    retur_beli_product_id t_guid,
    user_id t_guid,
    product_id t_guid,
    price t_price,
    quantity t_quantity,
    system_date timestamp without time zone DEFAULT now(),
    return_quantity t_quantity,
    purchase_item_id t_guid
);


ALTER TABLE t_purchase_return_item OWNER TO postgres;

--
-- Name: t_sales_return_item; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_sales_return_item(
    item_retur_sale_id t_guid NOT NULL,
    retur_sale_id t_guid,
    user_id t_guid,
    product_id t_guid,
    iselling_price t_price,
    quantity t_quantity,
    system_date timestamp without time zone DEFAULT now(),
    return_quantity t_quantity,
    item_sale_id t_guid
);


ALTER TABLE t_sales_return_item OWNER TO postgres;

--
-- Name: t_product_sales; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_product_sales(
    sale_id t_guid NOT NULL,
    user_id t_guid,
    customer_id t_guid,
    nota t_nota,
    date date,
    date_tempo date,
    tax t_price,
    discount t_price,
    total_invoice t_price,
    total_payment t_price,
    description t_description,
    system_date timestamp without time zone DEFAULT now(),
    retur_sale_id t_guid,
    shift_id t_guid,
    is_sdac t_bool,
    shipping_subdistrict t_address_panjang,
    shipping_city t_address_panjang,
    shipping_postal_code t_postal_code,
    shipping_kepada t_name,
    shipping_address t_address_panjang,
    shipping_phone t_phone,
    shipping_cost t_price,
    from_label1 t_description,
    from_label2 t_description,
    from_label3 t_description,
    from_label4 t_description,
    to_label1 t_address_panjang,
    to_label2 t_address_panjang,
    to_label3 t_address_panjang,
    to_label4 t_address_panjang,
    courier t_description,
    is_dropship boolean,
    shipping_country t_address_panjang,
    shipping_regency t_address_panjang,
    machine_id t_guid,
    payment_cash t_price,
    payment_card t_price,
    card_id t_guid,
    card_number t_nota,
    dropshipper_id t_guid
);


ALTER TABLE t_product_sales OWNER TO postgres;

--
-- Name: COLUMN t_product_sales.is_sdac; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN t_product_sales.is_sdac IS 'Sama dengan address customer';


--
-- Name: t_product_sales_jual_product_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE t_product_sales_jual_product_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_product_sales_jual_product_id_seq OWNER TO postgres;

--
-- Name: t_loan; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_loan (
    loan_id t_guid NOT NULL,
    employee_id t_guid,
    user_id t_guid,
    nota t_nota,
    date date,
    amount t_price,
    description t_description,
    system_date timestamp without time zone DEFAULT now(),
    total_payment t_price DEFAULT 0
);


ALTER TABLE t_loan OWNER TO postgres;

--
-- Name: t_loan_loan_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE t_loan_loan_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_loan_loan_id_seq OWNER TO postgres;

--
-- Name: t_logs; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_logs (
    log_id bigint DEFAULT nextval(('public.t_logs_log_id_seq'::text)::regclass) NOT NULL,
    level character varying(10),
    class_name character varying(200),
    method_name character varying(100),
    message character varying(100),
    new_value character varying(10000),
    old_value character varying(10000),
    exception character varying(10000),
    created_by character varying(50),
    log_date timestamp(0) without time zone DEFAULT now()
);


ALTER TABLE t_logs OWNER TO postgres;

--
-- Name: t_logs_log_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE t_logs_log_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_logs_log_id_seq OWNER TO postgres;

--
-- Name: t_machine; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_machine(
    machine_id t_guid NOT NULL,
    user_id t_guid,
    date date DEFAULT ('now'::text)::date,
    starting_balance t_price,
    cash_in t_price,
    system_date timestamp without time zone DEFAULT now(),
    shift_id t_guid,
    cash_out t_price
);


ALTER TABLE t_machine OWNER TO postgres;

--
-- Name: t_product_payable_payment; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_product_payable_payment (
    pembayaran_hutang_product_id t_guid NOT NULL,
    supplier_id t_guid,
    user_id t_guid,
    date date,
    description t_description,
    system_date timestamp without time zone DEFAULT now(),
    nota t_nota,
    is_cash t_bool
);


ALTER TABLE t_product_payable_payment OWNER TO postgres;

--
-- Name: t_product_payable_payment_pembayaran_hutang_product_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE t_product_payable_payment_pembayaran_hutang_product_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_product_payable_payment_pembayaran_hutang_product_id_seq OWNER TO postgres;

--
-- Name: t_pembayaran_loan; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_pembayaran_loan (
    pembayaran_loan_id t_guid NOT NULL,
    loan_id t_guid,
    gaji_employee_id t_guid,
    date date,
    amount t_price,
    description t_description,
    system_date timestamp without time zone DEFAULT now(),
    nota t_nota,
    user_id t_guid
);


ALTER TABLE t_pembayaran_loan OWNER TO postgres;

--
-- Name: t_pembayaran_loan_pembayaran_loan_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE t_pembayaran_loan_pembayaran_loan_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_pembayaran_loan_pembayaran_loan_id_seq OWNER TO postgres;

--
-- Name: t_product_receivable_payment; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_product_receivable_payment(
    pay_sale_id t_guid NOT NULL,
    customer_id t_guid,
    user_id t_guid,
    date date,
    description t_description,
    system_date timestamp without time zone DEFAULT now(),
    nota t_nota,
    is_cash t_bool
);


ALTER TABLE t_product_receivable_payment OWNER TO postgres;

--
-- Name: t_product_receivable_payment_pembayaran_piutang_product_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE t_product_receivable_payment_pembayaran_piutang_product_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_product_receivable_payment_pembayaran_piutang_product_id_seq OWNER TO postgres;

--
-- Name: t_expence; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_expence(
    expense_id t_guid NOT NULL,
    user_id t_guid,
    nota t_nota,
    date date,
    total t_price,
    description t_description,
    system_date timestamp without time zone DEFAULT now()
);


ALTER TABLE t_expence OWNER TO postgres;

--
-- Name: t_expence_pengeluaran_biaya_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE t_expence_pengeluaran_biaya_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_expence_pengeluaran_biaya_id_seq OWNER TO postgres;

--
-- Name: t_stock_adjustment; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_stock_adjustment (
    stock_adjustment_id t_guid NOT NULL,
    product_id t_guid,
    adjustment_reason_id t_guid,
    date date,
    stock_addition t_quantity,
    stock_reduction t_quantity,
    description t_description,
    system_date timestamp without time zone DEFAULT now(),
    penambahan_warehouse_stock t_quantity,
    pengurangan_warehouse_stock t_quantity
);
ALTER TABLE ONLY t_stock_adjustment ALTER COLUMN adjustment_reason_id SET STATISTICS 0;


ALTER TABLE t_stock_adjustment OWNER TO postgres;

--
-- Name: t_purchase_return; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_purchase_return (
    retur_beli_product_id t_guid NOT NULL,
    beli_product_id t_guid,
    user_id t_guid,
    supplier_id t_guid,
    nota t_nota,
    date date,
    description t_description,
    system_date timestamp without time zone DEFAULT now(),
    total_invoice t_price
);


ALTER TABLE t_purchase_return OWNER TO postgres;

--
-- Name: t_purchase_return_retur_beli_product_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE t_purchase_return_retur_beli_product_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_purchase_return_retur_beli_product_id_seq OWNER TO postgres;

--
-- Name: t_sales_return; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_sales_return (
    retur_sale_id t_guid NOT NULL,
    sale_id t_guid,
    user_id t_guid,
    customer_id t_guid,
    nota t_nota,
    date date,
    description t_description,
    system_date timestamp without time zone DEFAULT now(),
    total_invoice t_price
);


ALTER TABLE t_sales_return OWNER TO postgres;

--
-- Name: t_sales_return_retur_jual_product_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE t_sales_return_retur_jual_product_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_sales_return_retur_jual_product_id_seq OWNER TO postgres;

--
-- Name: m_adjustment_reason_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_adjustment_reason
    ADD CONSTRAINT m_adjustment_reason_pkey PRIMARY KEY (stock_adjustment_reason_id);


--
-- Name: m_customer_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_customer
    ADD CONSTRAINT m_customer_pkey PRIMARY KEY (customer_id);


--
-- Name: m_database_version_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_database_version
    ADD CONSTRAINT m_database_version_pkey PRIMARY KEY (version_number);


--
-- Name: m_dropshipper_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_dropshipper
    ADD CONSTRAINT m_dropshipper_pkey PRIMARY KEY (dropshipper_id);


--
-- Name: m_mini_pos_invoice_footer_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_mini_pos_invoice_footer
    ADD CONSTRAINT m_mini_pos_invoice_footer_pkey PRIMARY KEY (footer_invoice_id);


--
-- Name: m_category_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_category
    ADD CONSTRAINT m_category_pkey PRIMARY KEY (category_id);


--
-- Name: m_wholesale_price_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_wholesale_price
    ADD CONSTRAINT m_wholesale_price_pkey PRIMARY KEY (wholesale_price_id);


--
-- Name: m_invoice_header_mini_pos_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_invoice_header_mini_pos
    ADD CONSTRAINT m_invoice_header_mini_pos_pkey PRIMARY KEY (header_invoice_id);


--
-- Name: m_invoice_header_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_invoice_header
    ADD CONSTRAINT m_invoice_header_pkey PRIMARY KEY (header_invoice_id);


--
-- Name: m_item_menu_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_item_menu
    ADD CONSTRAINT m_item_menu_pkey PRIMARY KEY (item_menu_id);


--
-- Name: m_job_titles_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_job_titles
    ADD CONSTRAINT m_job_titles_pkey PRIMARY KEY (job_titles_id);


--
-- Name: m_expense_type_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_expense_type
    ADD CONSTRAINT m_expense_type_pkey PRIMARY KEY (expense_type_id);


--
-- Name: m_regency2_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_regency2
    ADD CONSTRAINT m_regency2_pkey PRIMARY KEY (regency_id);


--
-- Name: m_regency_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_regency
    ADD CONSTRAINT m_regency_pkey PRIMARY KEY (regency_id);


--
-- Name: m_card_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_card
    ADD CONSTRAINT m_card_pkey PRIMARY KEY (card_id);


--
-- Name: m_employee_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_employee
    ADD CONSTRAINT m_employee_pkey PRIMARY KEY (employee_id);


--
-- Name: m_subdistrict_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_subdistrict
    ADD CONSTRAINT m_subdistrict_pkey PRIMARY KEY (subdistrict_id);


--
-- Name: m_invoice_label_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_invoice_label
    ADD CONSTRAINT m_invoice_label_pkey PRIMARY KEY (invoice_label_id);


--
-- Name: m_menu_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_menu
    ADD CONSTRAINT m_menu_pkey PRIMARY KEY (menu_id);


--
-- Name: m_user_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_user
    ADD CONSTRAINT m_user_pkey PRIMARY KEY (user_id);


--
-- Name: m_invoice_prefix_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_invoice_prefix
    ADD CONSTRAINT m_invoice_prefix_pkey PRIMARY KEY (prefix_invoice_id);


--
-- Name: m_product_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_product
    ADD CONSTRAINT m_product_pkey PRIMARY KEY (product_id);


--
-- Name: m_profile_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_profile
    ADD CONSTRAINT m_profile_pkey PRIMARY KEY (profile_id);


--
-- Name: m_provinsi2_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_province2
    ADD CONSTRAINT m_provinsi2_pkey PRIMARY KEY (province_id);


--
-- Name: m_provinsi_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_province
    ADD CONSTRAINT m_provinsi_pkey PRIMARY KEY (province_id);


--
-- Name: m_role_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_role
    ADD CONSTRAINT m_role_pkey PRIMARY KEY (role_id);


--
-- Name: m_role_privilege_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_role_privilege
    ADD CONSTRAINT m_role_privilege_pkey PRIMARY KEY (role_id, menu_id, grant_id);


--
-- Name: m_application_setting_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_application_setting
    ADD CONSTRAINT m_application_setting_pkey PRIMARY KEY (application_setting_id);


--
-- Name: m_shift_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_shift
    ADD CONSTRAINT m_shift_pkey PRIMARY KEY (shift_id);


--
-- Name: m_supplier_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_supplier
    ADD CONSTRAINT m_supplier_pkey PRIMARY KEY (supplier_id);


--
-- Name: t_purchase_product_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_purchase_product
    ADD CONSTRAINT t_purchase_product_pkey PRIMARY KEY (beli_product_id);


--
-- Name: t_employee_salary_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_employee_salary
    ADD CONSTRAINT t_employee_salary_pkey PRIMARY KEY (gaji_employee_id);


--
-- Name: t_purchase_order_item_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_purchase_order_item
    ADD CONSTRAINT t_purchase_order_item_pkey PRIMARY KEY (item_beli_product_id);


--
-- Name: t_item_jual_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_sales_order_item
    ADD CONSTRAINT t_item_jual_pkey PRIMARY KEY (item_sale_id);


--
-- Name: t_debt_payment_item_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_debt_payment_item
    ADD CONSTRAINT t_debt_payment_item_pkey PRIMARY KEY (item_pembayaran_hutang_product_id);


--
-- Name: t_item_pembayaran_piutang_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_credit_payment_item
    ADD CONSTRAINT t_item_pembayaran_piutang_pkey PRIMARY KEY (pay_sale_item_id);


--
-- Name: t_item_pengeluaran_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_expense_item
    ADD CONSTRAINT t_item_pengeluaran_pkey PRIMARY KEY (expense_item_id);


--
-- Name: t_purchase_return_item_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_purchase_return_item
    ADD CONSTRAINT t_purchase_return_item_pkey PRIMARY KEY (item_retur_beli_product_id);


--
-- Name: t_item_retur_jual_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_sales_return_item
    ADD CONSTRAINT t_item_retur_jual_pkey PRIMARY KEY (item_retur_sale_id);


--
-- Name: t_jual_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_product_sales
    ADD CONSTRAINT t_jual_pkey PRIMARY KEY (sale_id);


--
-- Name: t_loan_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_loan
    ADD CONSTRAINT t_loan_pkey PRIMARY KEY (loan_id);


--
-- Name: t_logs_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_logs
    ADD CONSTRAINT t_logs_pkey PRIMARY KEY (log_id);


--
-- Name: t_machine_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_machine
    ADD CONSTRAINT t_machine_pkey PRIMARY KEY (machine_id);


--
-- Name: t_pembayaran_bon_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_pembayaran_loan
    ADD CONSTRAINT t_pembayaran_bon_pkey PRIMARY KEY (pembayaran_loan_id);


--
-- Name: t_product_payable_payment_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_product_payable_payment
    ADD CONSTRAINT t_product_payable_payment_pkey PRIMARY KEY (pembayaran_hutang_product_id);


--
-- Name: t_pembayaran_piutang_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_product_receivable_payment
    ADD CONSTRAINT t_pembayaran_piutang_pkey PRIMARY KEY (pay_sale_id);


--
-- Name: t_pengeluaran_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_expence
    ADD CONSTRAINT t_pengeluaran_pkey PRIMARY KEY (expense_id);


--
-- Name: t_stock_adjustment_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_stock_adjustment
    ADD CONSTRAINT t_stock_adjustment_pkey PRIMARY KEY (stock_adjustment_id);


--
-- Name: t_purchase_return_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_purchase_return
    ADD CONSTRAINT t_purchase_return_pkey PRIMARY KEY (retur_beli_product_id);


--
-- Name: t_retur_jual_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_sales_return
    ADD CONSTRAINT t_retur_jual_pkey PRIMARY KEY (retur_sale_id);


--
-- Name: m_customer_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX m_customer_idx ON m_customer USING btree (name_customer);


--
-- Name: m_regency2_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX m_regency2_idx ON m_regency2 USING btree (province_id);


--
-- Name: m_regency2_idx1; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX m_regency2_idx1 ON m_regency2 USING btree (name_regency);


--
-- Name: m_regency_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX m_regency_idx ON m_regency USING btree (name_regency);


--
-- Name: m_subdistrict_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX m_subdistrict_idx ON m_subdistrict USING btree (regency_id);


--
-- Name: m_subdistrict_idx1; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX m_subdistrict_idx1 ON m_subdistrict USING btree (name_subdistrict);


--
-- Name: m_product_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX m_product_idx ON m_product USING btree (product_name);


--
-- Name: m_product_idx1; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX m_product_idx1 ON m_product USING btree (product_code);


--
-- Name: m_product_idx2; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX m_product_idx2 ON m_product USING btree (category_id);


--
-- Name: m_product_idx3; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX m_product_idx3 ON m_product USING btree (is_active);


--
-- Name: m_provinsi2_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX m_provinsi2_idx ON m_province2 USING btree (name_province);


--
-- Name: m_province_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX m_province_idx ON m_province USING btree (name_province);


--
-- Name: m_supplier_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX m_supplier_idx ON m_supplier USING btree (name_supplier);


--
-- Name: t_beli_product_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_beli_product_idx ON t_purchase_product USING btree (date);


--
-- Name: t_beli_product_idx1; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_beli_product_idx1 ON t_purchase_product USING btree (nota);


--
-- Name: t_beli_product_idx2; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_beli_product_idx2 ON t_purchase_product USING btree (date_tempo);


--
-- Name: t_beli_product_idx3; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_beli_product_idx3 ON t_purchase_product USING btree (supplier_id);


--
-- Name: t_beli_product_idx4; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_beli_product_idx4 ON t_purchase_product USING btree (user_id);


--
-- Name: t_gaji_employee_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_gaji_employee_idx ON t_employee_salary USING btree (month, year);


--
-- Name: t_gaji_employee_idx1; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_gaji_employee_idx1 ON t_employee_salary USING btree (date);


--
-- Name: t_gaji_employee_idx2; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_gaji_employee_idx2 ON t_employee_salary USING btree (nota);


--
-- Name: t_jual_product_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_jual_product_idx ON t_product_sales USING btree (nota);


--
-- Name: t_jual_product_idx1; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_jual_product_idx1 ON t_product_sales USING btree (date);


--
-- Name: t_jual_product_idx2; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_jual_product_idx2 ON t_product_sales USING btree (date_tempo);


--
-- Name: t_jual_product_idx3; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_jual_product_idx3 ON t_product_sales USING btree (customer_id);


--
-- Name: t_jual_product_idx4; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_jual_product_idx4 ON t_product_sales USING btree (user_id);


--
-- Name: t_loan_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_loan_idx ON t_loan USING btree (date);


--
-- Name: t_loan_idx1; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_loan_idx1 ON t_loan USING btree (nota);


--
-- Name: t_machine_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_machine_idx ON t_machine USING btree (date);


--
-- Name: t_machine_idx1; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_machine_idx1 ON t_machine USING btree (user_id);


--
-- Name: t_pembayaran_hutang_product_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_pembayaran_hutang_product_idx ON t_product_payable_payment USING btree (date);


--
-- Name: t_pembayaran_hutang_product_idx1; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_pembayaran_hutang_product_idx1 ON t_product_payable_payment USING btree (nota);


--
-- Name: t_pembayaran_hutang_product_idx2; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_pembayaran_hutang_product_idx2 ON t_product_payable_payment USING btree (supplier_id);


--
-- Name: t_pembayaran_loan_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_pembayaran_loan_idx ON t_pembayaran_loan USING btree (date);


--
-- Name: t_pembayaran_loan_idx1; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_pembayaran_loan_idx1 ON t_pembayaran_loan USING btree (nota);


--
-- Name: t_pembayaran_piutang_product_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_pembayaran_piutang_product_idx ON t_product_receivable_payment USING btree (date);


--
-- Name: t_pembayaran_piutang_product_idx1; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_pembayaran_piutang_product_idx1 ON t_product_receivable_payment USING btree (nota);


--
-- Name: t_pembayaran_piutang_product_idx2; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_pembayaran_piutang_product_idx2 ON t_product_receivable_payment USING btree (customer_id);


--
-- Name: t_expence_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_expence_idx ON t_expence USING btree (date);


--
-- Name: t_expence_idx1; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_expence_idx1 ON t_expence USING btree (nota);


--
-- Name: t_stock_adjustment_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_stock_adjustment_idx ON t_stock_adjustment USING btree (date);


--
-- Name: t_retur_beli_product_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_retur_beli_product_idx ON t_purchase_return USING btree (date);


--
-- Name: t_retur_beli_product_idx1; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_retur_beli_product_idx1 ON t_purchase_return USING btree (nota);


--
-- Name: t_retur_jual_product_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_retur_jual_product_idx ON t_sales_return USING btree (date);


--
-- Name: t_retur_jual_product_idx1; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_retur_jual_product_idx1 ON t_sales_return USING btree (nota);


--
-- Name: tr_hapus_header_ad; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_hapus_header_ad AFTER DELETE ON t_debt_payment_item FOR EACH ROW EXECUTE PROCEDURE f_delete_payable_product_header();


--
-- Name: tr_hapus_header_ad; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_hapus_header_ad AFTER DELETE ON t_credit_payment_item FOR EACH ROW EXECUTE PROCEDURE f_delete_receivable_product_header();


--
-- Name: tr_log_last_update; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_log_last_update AFTER UPDATE OF iselling_price ON m_product FOR EACH ROW EXECUTE PROCEDURE fn_log_last_update();


--
-- Name: tr_penyesuaian_stok_aiud; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_penyesuaian_stok_aiud AFTER INSERT OR DELETE OR UPDATE ON t_stock_adjustment FOR EACH ROW EXECUTE PROCEDURE f_adjust_product_stock_aiud();


--
-- Name: tr_update_return_quantity_beli; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_update_return_quantity_beli AFTER INSERT OR DELETE OR UPDATE ON t_purchase_return_item FOR EACH ROW EXECUTE PROCEDURE f_update_purchase_return_quantity();


--
-- Name: tr_update_return_quantity_jual; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_update_return_quantity_jual AFTER INSERT OR DELETE OR UPDATE ON t_sales_return_item FOR EACH ROW EXECUTE PROCEDURE f_update_return_quantity_sales();


--
-- Name: tr_update_pelunasan_beli_produk_aiud; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_update_pelunasan_beli_produk_aiud AFTER INSERT OR DELETE OR UPDATE ON t_debt_payment_item FOR EACH ROW EXECUTE PROCEDURE f_update_purchase_payment();


--
-- Name: tr_update_pelunasan_jual_produk_aiud; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_update_pelunasan_jual_produk_aiud AFTER INSERT OR DELETE OR UPDATE ON t_credit_payment_item FOR EACH ROW EXECUTE PROCEDURE f_update_sales_payment();


--
-- Name: tr_update_pelunasan_loan_aiud; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_update_pelunasan_loan_aiud AFTER INSERT OR DELETE OR UPDATE ON t_pembayaran_loan FOR EACH ROW EXECUTE PROCEDURE f_update_loan_payment();


--
-- Name: tr_update_stok_produk_aiud; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_update_stok_produk_aiud AFTER INSERT OR DELETE OR UPDATE ON t_purchase_order_item FOR EACH ROW EXECUTE PROCEDURE f_add_product_stock();


--
-- Name: tr_update_stok_produk_aiud; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_update_stok_produk_aiud AFTER INSERT OR DELETE OR UPDATE ON t_sales_order_item FOR EACH ROW EXECUTE PROCEDURE f_reduce_product_stock();


--
-- Name: tr_update_total_beli_produk_aiud; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_update_total_beli_produk_aiud AFTER INSERT OR DELETE OR UPDATE ON t_purchase_order_item FOR EACH ROW EXECUTE PROCEDURE f_update_total_purchase();


--
-- Name: tr_update_total_debt_produk_supplier; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_update_total_debt_produk_supplier AFTER INSERT OR DELETE OR UPDATE ON t_purchase_product FOR EACH ROW EXECUTE PROCEDURE f_update_supplier_total_debt();


--
-- Name: tr_update_total_jual_produk_aiud; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_update_total_jual_produk_aiud AFTER INSERT OR DELETE OR UPDATE ON t_sales_order_item FOR EACH ROW EXECUTE PROCEDURE f_update_total_sales();


--
-- Name: tr_update_total_loan_karyawan; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_update_total_loan_karyawan AFTER INSERT OR DELETE OR UPDATE ON t_loan FOR EACH ROW EXECUTE PROCEDURE f_update_employee_total_loan();


--
-- Name: tr_update_total_pengeluaran_aiud; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_update_total_pengeluaran_aiud AFTER INSERT OR DELETE OR UPDATE ON t_expense_item FOR EACH ROW EXECUTE PROCEDURE f_update_total_expenses();


--
-- Name: tr_update_total_credit_customer; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_update_total_credit_customer AFTER INSERT OR DELETE OR UPDATE ON t_product_sales FOR EACH ROW EXECUTE PROCEDURE f_update_customer_total_credit();


--
-- Name: tr_update_total_retur_beli_aiud; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_update_total_retur_beli_aiud AFTER INSERT OR DELETE OR UPDATE ON t_purchase_return_item FOR EACH ROW EXECUTE PROCEDURE f_update_purchase_return_total_aiud();


--
-- Name: tr_update_total_retur_produk_aiud; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_update_total_retur_produk_aiud AFTER INSERT OR DELETE OR UPDATE ON t_sales_return_item FOR EACH ROW EXECUTE PROCEDURE f_update_sales_return_total_aiud();


--
-- Name: m_wholesale_price_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY m_wholesale_price
    ADD CONSTRAINT m_wholesale_price_fk FOREIGN KEY (product_id) REFERENCES m_product(product_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: m_item_menu_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY m_item_menu
    ADD CONSTRAINT m_item_menu_fk FOREIGN KEY (menu_id) REFERENCES m_menu(menu_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: m_regency2_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY m_regency2
    ADD CONSTRAINT m_regency2_fk FOREIGN KEY (province_id) REFERENCES m_province2(province_id) ON UPDATE CASCADE;


--
-- Name: m_regency_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY m_regency
    ADD CONSTRAINT m_regency_fk FOREIGN KEY (province_id) REFERENCES m_province(province_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: m_employee_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY m_employee
    ADD CONSTRAINT m_employee_fk FOREIGN KEY (job_titles_id) REFERENCES m_job_titles(job_titles_id) ON UPDATE CASCADE;


--
-- Name: m_subdistrict_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY m_subdistrict
    ADD CONSTRAINT m_subdistrict_fk FOREIGN KEY (regency_id) REFERENCES m_regency2(regency_id) ON UPDATE CASCADE;


--
-- Name: m_user_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY m_user
    ADD CONSTRAINT m_user_fk FOREIGN KEY (role_id) REFERENCES m_role(role_id) ON UPDATE CASCADE;


--
-- Name: m_product_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY m_product
    ADD CONSTRAINT m_product_fk FOREIGN KEY (category_id) REFERENCES m_category(category_id) ON UPDATE CASCADE;


--
-- Name: m_role_privilege_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY m_role_privilege
    ADD CONSTRAINT m_role_privilege_fk FOREIGN KEY (menu_id) REFERENCES m_menu(menu_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: m_role_privilege_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY m_role_privilege
    ADD CONSTRAINT m_role_privilege_fk1 FOREIGN KEY (role_id) REFERENCES m_role(role_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: t_purchase_product_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_purchase_product
    ADD CONSTRAINT t_purchase_product_fk FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_purchase_product_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_purchase_product
    ADD CONSTRAINT t_purchase_product_fk1 FOREIGN KEY (supplier_id) REFERENCES m_supplier(supplier_id) ON UPDATE CASCADE;


--
-- Name: t_purchase_product_fk2; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_purchase_product
    ADD CONSTRAINT t_purchase_product_fk2 FOREIGN KEY (retur_beli_product_id) REFERENCES t_purchase_return(retur_beli_product_id) ON UPDATE CASCADE ON DELETE SET NULL;


--
-- Name: t_bon_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_loan
    ADD CONSTRAINT t_bon_fk FOREIGN KEY (employee_id) REFERENCES m_employee(employee_id) ON UPDATE CASCADE;


--
-- Name: t_bon_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_loan
    ADD CONSTRAINT t_bon_fk1 FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_employee_salary_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_employee_salary
    ADD CONSTRAINT t_employee_salary_fk FOREIGN KEY (employee_id) REFERENCES m_employee(employee_id) ON UPDATE CASCADE;


--
-- Name: t_employee_salary_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_employee_salary
    ADD CONSTRAINT t_employee_salary_fk1 FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_purchase_order_item_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_purchase_order_item
    ADD CONSTRAINT t_purchase_order_item_fk FOREIGN KEY (beli_product_id) REFERENCES t_purchase_product(beli_product_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: t_purchase_order_item_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_purchase_order_item
    ADD CONSTRAINT t_purchase_order_item_fk1 FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_purchase_order_item_fk2; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_purchase_order_item
    ADD CONSTRAINT t_purchase_order_item_fk2 FOREIGN KEY (product_id) REFERENCES m_product(product_id) ON UPDATE CASCADE;


--
-- Name: t_item_jual_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_sales_order_item
    ADD CONSTRAINT t_item_jual_fk FOREIGN KEY (sale_id) REFERENCES t_product_sales(sale_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: t_item_jual_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_sales_order_item
    ADD CONSTRAINT t_item_jual_fk1 FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_item_jual_fk2; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_sales_order_item
    ADD CONSTRAINT t_item_jual_fk2 FOREIGN KEY (product_id) REFERENCES m_product(product_id) ON UPDATE CASCADE;


--
-- Name: t_debt_payment_item_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_debt_payment_item
    ADD CONSTRAINT t_debt_payment_item_fk FOREIGN KEY (pembayaran_hutang_product_id) REFERENCES t_product_payable_payment(pembayaran_hutang_product_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: t_debt_payment_item_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_debt_payment_item
    ADD CONSTRAINT t_debt_payment_item_fk1 FOREIGN KEY (beli_product_id) REFERENCES t_purchase_product(beli_product_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: t_item_pembayaran_piutang_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_credit_payment_item
    ADD CONSTRAINT t_item_pembayaran_piutang_fk FOREIGN KEY (pay_sale_id) REFERENCES t_product_receivable_payment(pay_sale_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: t_item_pembayaran_piutang_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_credit_payment_item
    ADD CONSTRAINT t_item_pembayaran_piutang_fk1 FOREIGN KEY (sale_id) REFERENCES t_product_sales(sale_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: t_item_pengeluaran_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_expense_item
    ADD CONSTRAINT t_item_pengeluaran_fk FOREIGN KEY (expense_id) REFERENCES t_expence(expense_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: t_item_pengeluaran_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_expense_item
    ADD CONSTRAINT t_item_pengeluaran_fk1 FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_item_pengeluaran_fk2; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_expense_item
    ADD CONSTRAINT t_item_pengeluaran_fk2 FOREIGN KEY (expense_type_id) REFERENCES m_expense_type(expense_type_id) ON UPDATE CASCADE;


--
-- Name: t_purchase_return_item_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_purchase_return_item
    ADD CONSTRAINT t_purchase_return_item_fk FOREIGN KEY (retur_beli_product_id) REFERENCES t_purchase_return(retur_beli_product_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: t_purchase_return_item_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_purchase_return_item
    ADD CONSTRAINT t_purchase_return_item_fk1 FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_purchase_return_item_fk2; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_purchase_return_item
    ADD CONSTRAINT t_purchase_return_item_fk2 FOREIGN KEY (product_id) REFERENCES m_product(product_id) ON UPDATE CASCADE;


--
-- Name: t_purchase_return_item_fk3; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_purchase_return_item
    ADD CONSTRAINT t_purchase_return_item_fk3 FOREIGN KEY (purchase_item_id) REFERENCES t_purchase_order_item(item_beli_product_id) ON UPDATE CASCADE;


--
-- Name: t_item_retur_jual_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_sales_return_item
    ADD CONSTRAINT t_item_retur_jual_fk FOREIGN KEY (retur_sale_id) REFERENCES t_sales_return(retur_sale_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: t_item_retur_jual_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_sales_return_item
    ADD CONSTRAINT t_item_retur_jual_fk1 FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_item_retur_jual_fk2; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_sales_return_item
    ADD CONSTRAINT t_item_retur_jual_fk2 FOREIGN KEY (product_id) REFERENCES m_product(product_id) ON UPDATE CASCADE;


--
-- Name: t_item_retur_jual_fk3; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_sales_return_item
    ADD CONSTRAINT t_item_retur_jual_fk3 FOREIGN KEY (item_sale_id) REFERENCES t_sales_order_item(item_sale_id) ON UPDATE CASCADE;


--
-- Name: t_jual_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_product_sales
    ADD CONSTRAINT t_jual_fk FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_jual_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_product_sales
    ADD CONSTRAINT t_jual_fk1 FOREIGN KEY (customer_id) REFERENCES m_customer(customer_id) ON UPDATE CASCADE;


--
-- Name: t_jual_fk2; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_product_sales
    ADD CONSTRAINT t_jual_fk2 FOREIGN KEY (retur_sale_id) REFERENCES t_sales_return(retur_sale_id) ON UPDATE CASCADE ON DELETE SET NULL;


--
-- Name: t_jual_fk3; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_product_sales
    ADD CONSTRAINT t_jual_fk3 FOREIGN KEY (shift_id) REFERENCES m_shift(shift_id) ON UPDATE CASCADE;


--
-- Name: t_jual_fk4; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_product_sales
    ADD CONSTRAINT t_jual_fk4 FOREIGN KEY (dropshipper_id) REFERENCES m_dropshipper(dropshipper_id) ON UPDATE CASCADE;


--
-- Name: t_jual_fk5; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_product_sales
    ADD CONSTRAINT t_jual_fk5 FOREIGN KEY (card_id) REFERENCES m_card(card_id) ON UPDATE CASCADE;


--
-- Name: t_machine_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_machine
    ADD CONSTRAINT t_machine_fk FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_pembayaran_bon_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_pembayaran_loan
    ADD CONSTRAINT t_pembayaran_bon_fk1 FOREIGN KEY (gaji_employee_id) REFERENCES t_employee_salary(gaji_employee_id) ON UPDATE CASCADE;


--
-- Name: t_pembayaran_bon_fk2; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_pembayaran_loan
    ADD CONSTRAINT t_pembayaran_bon_fk2 FOREIGN KEY (loan_id) REFERENCES t_loan(loan_id) ON UPDATE CASCADE;


--
-- Name: t_product_payable_payment_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_product_payable_payment
    ADD CONSTRAINT t_product_payable_payment_fk FOREIGN KEY (supplier_id) REFERENCES m_supplier(supplier_id) ON UPDATE CASCADE;


--
-- Name: t_product_payable_payment_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_product_payable_payment
    ADD CONSTRAINT t_product_payable_payment_fk1 FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_pembayaran_loan_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_pembayaran_loan
    ADD CONSTRAINT t_pembayaran_loan_fk FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_pembayaran_piutang_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_product_receivable_payment
    ADD CONSTRAINT t_pembayaran_piutang_fk FOREIGN KEY (customer_id) REFERENCES m_customer(customer_id) ON UPDATE CASCADE;


--
-- Name: t_pembayaran_piutang_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_product_receivable_payment
    ADD CONSTRAINT t_pembayaran_piutang_fk1 FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_pengeluaran_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_expence
    ADD CONSTRAINT t_pengeluaran_fk FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_stock_adjustment_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_stock_adjustment
    ADD CONSTRAINT t_stock_adjustment_fk FOREIGN KEY (product_id) REFERENCES m_product(product_id) ON UPDATE CASCADE;


--
-- Name: t_stock_adjustment_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_stock_adjustment
    ADD CONSTRAINT t_stock_adjustment_fk1 FOREIGN KEY (adjustment_reason_id) REFERENCES m_adjustment_reason(stock_adjustment_reason_id) ON UPDATE CASCADE;


--
-- Name: t_purchase_return_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_purchase_return
    ADD CONSTRAINT t_purchase_return_fk FOREIGN KEY (beli_product_id) REFERENCES t_purchase_product(beli_product_id) ON UPDATE CASCADE ON DELETE SET NULL;


--
-- Name: t_purchase_return_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_purchase_return
    ADD CONSTRAINT t_purchase_return_fk1 FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_purchase_return_fk2; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_purchase_return
    ADD CONSTRAINT t_purchase_return_fk2 FOREIGN KEY (supplier_id) REFERENCES m_supplier(supplier_id) ON UPDATE CASCADE;


--
-- Name: t_retur_jual_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_sales_return
    ADD CONSTRAINT t_retur_jual_fk FOREIGN KEY (sale_id) REFERENCES t_product_sales(sale_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: t_retur_jual_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_sales_return
    ADD CONSTRAINT t_retur_jual_fk1 FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_retur_jual_fk2; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_sales_return
    ADD CONSTRAINT t_retur_jual_fk2 FOREIGN KEY (customer_id) REFERENCES m_customer(customer_id) ON UPDATE CASCADE;


--
-- Name: public; Type: ACL; Schema: -; Owner: postgres
--

REVOKE ALL ON SCHEMA public FROM PUBLIC;
REVOKE ALL ON SCHEMA public FROM postgres;
GRANT ALL ON SCHEMA public TO postgres;
GRANT ALL ON SCHEMA public TO PUBLIC;


--
-- PostgreSQL database dump complete
--

