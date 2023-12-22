

CREATE DOMAIN t_address_long AS character varying(250);


ALTER DOMAIN t_address_long OWNER TO postgres;



CREATE DOMAIN t_quotation AS character varying(20);


ALTER DOMAIN t_quotation OWNER TO postgres;



CREATE TABLE t_sales_quotation (
    quotation_id t_guid NOT NULL,
    user_id t_guid,
    customer_id t_guid,
    quotation t_quotation,
    date date,
    valid_until date,
    tax t_price,
    discount t_price,
    total_quotation t_price,
    description t_description,
    system_date timestamp without time zone DEFAULT now(),
    shipping_subdistrict t_address_long,
    shipping_city t_address_long,
    shipping_postal_code t_postal_code,
    shipping_address t_address_long,
    shipping_phone t_phone,
    shipping_cost t_price,
    shipping_to t_name,
    shipping_village text,
    from_label1 t_description,
    from_label2 t_description,
    from_label3 t_description,
    from_label4 t_description,
    to_label1 t_address_long,
    to_label2 t_address_long,
    to_label3 t_address_long,
    to_label4 t_address_long,
    courier t_description,
    is_dropship boolean,
    shipping_country t_address_long,
    shipping_regency t_address_long,
    machine_id t_guid,
    payment_cash t_price,
    payment_card t_price,
    card_id t_guid,
    card_number t_invoice,
    dropshipper_id t_guid
);

ALTER TABLE t_sales_quotation OWNER TO postgres;

CREATE TABLE t_sales_quotation_item (
    quotation_item_id t_guid NOT NULL,
    quotation_id t_guid,
    user_id t_guid,
    product_id t_guid,
    purchase_price t_price,
    selling_price t_price,
    quantity t_quantity,
    discount t_quantity,
    system_date timestamp without time zone DEFAULT now(),
    description t_description
);

ALTER TABLE t_sales_quotation_item OWNER TO postgres;

-- Primary Key Constraint
ALTER TABLE t_sales_quotation ADD PRIMARY KEY (quotation_id);

-- Foreign Key Constraint
ALTER TABLE t_sales_quotation
ADD CONSTRAINT fk_sales_quotation_user_id
FOREIGN KEY (user_id)
REFERENCES m_user(user_id);



-- Primary Key Constraint
ALTER TABLE t_sales_quotation_item ADD PRIMARY KEY (quotation_item_id);

-- Foreign Key Constraint
ALTER TABLE t_sales_quotation_item
ADD CONSTRAINT fk_sales_quotation_item_quotation_id
FOREIGN KEY (quotation_id)
REFERENCES t_sales_quotation(quotation_id);

-- Foreign Key Constraint
ALTER TABLE t_sales_quotation
ADD CONSTRAINT fk_sales_quotation_customer_id
FOREIGN KEY (customer_id)
REFERENCES m_customer(customer_id);

-- Foreign Key Constraint
ALTER TABLE t_sales_quotation_item
ADD CONSTRAINT fk_sales_quotation_item_product_id
FOREIGN KEY (product_id)
REFERENCES m_product(product_id);

-- Foreign Key Constraint
ALTER TABLE t_sales_quotation_item
ADD CONSTRAINT fk_sales_quotation_item_user_id
FOREIGN KEY (user_id)
REFERENCES m_user(user_id);

--forighn key constraint

ALTER TABLE t_sales_quotation
ADD CONSTRAINT fk_sales_quotation_machine_id
FOREIGN KEY (machine_id)
REFERENCES t_machine(machine_id);

--forighn key constraint

ALTER TABLE t_sales_quotation
ADD CONSTRAINT fk_sales_quotation_card_id
FOREIGN KEY (card_id)
REFERENCES m_card(card_id);


--forighn key constraint

ALTER TABLE t_sales_quotation
ADD CONSTRAINT fk_sales_quotation_dropshipper_id
FOREIGN KEY (dropshipper_id)
REFERENCES m_dropshipper(dropshipper_id);

INSERT INTO m_menu (menu_id, name_menu, menu_title, parent_id, order_number, is_active, name_form, is_enabled)
VALUES ('c9eb7f5d-0373-48b6-9b15-14e8349d9e1f', 'mnuSalesQuotation', 'Sales Quotation', '1afdbd3e-7e82-4e64-9c35-2d010a141b2f', '5', true, 'FrmListSalesQuotation', true);



COPY public.m_item_menu (item_menu_id, menu_id, grant_id, description) FROM stdin;
f67065c9-ab17-468e-b54a-e8ff41f07bb1	c9eb7f5d-0373-48b6-9b15-14e8349d9e1f	0	Melihat Data
1181f4d2-e789-493d-b86d-46ebad5da071	c9eb7f5d-0373-48b6-9b15-14e8349d9e1f	1	Tambah Data
412f64f8-5011-4a43-9fd8-c1136c5f06f8	c9eb7f5d-0373-48b6-9b15-14e8349d9e1f	2	Edit Data
7fa2bbd6-8c53-4a63-80b9-e305e20494c6	c9eb7f5d-0373-48b6-9b15-14e8349d9e1f	3	Hapus Data
\.



COPY public.m_role_privilege (role_id, menu_id, grant_id, is_grant) FROM stdin;
11dc1faf-2c66-4525-932d-a90e24da8987	c9eb7f5d-0373-48b6-9b15-14e8349d9e1f	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	c9eb7f5d-0373-48b6-9b15-14e8349d9e1f	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	c9eb7f5d-0373-48b6-9b15-14e8349d9e1f	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	c9eb7f5d-0373-48b6-9b15-14e8349d9e1f	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	c9eb7f5d-0373-48b6-9b15-14e8349d9e1f	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	c9eb7f5d-0373-48b6-9b15-14e8349d9e1f	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	c9eb7f5d-0373-48b6-9b15-14e8349d9e1f	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	c9eb7f5d-0373-48b6-9b15-14e8349d9e1f	3	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	c9eb7f5d-0373-48b6-9b15-14e8349d9e1f	0	f
\.



CREATE SEQUENCE t_sales_quotation_sales_quotation_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_sales_quotation_sales_quotation_id_seq OWNER TO postgres;



ALTER TABLE t_sales_quotation ADD COLUMN return_quantity integer;

ALTER TABLE t_sales_quotation ADD COLUMN total_payment t_price;

ALTER TABLE t_sales_quotation ADD COLUMN is_sdac BOOLEAN;


ALTER TABLE t_product_sales
ADD COLUMN quotation_id t_guid;


ALTER TABLE t_product_sales
ADD CONSTRAINT fk_quotation_id
FOREIGN KEY (quotation_id)
REFERENCES t_sales_quotation (quotation_id);



CREATE DOMAIN t_delivery AS character varying(20);


ALTER DOMAIN t_delivery OWNER TO postgres;

CREATE TABLE t_delivery_notes (
    delivery_id t_guid NOT NULL,
    user_id t_guid,
    customer_id t_guid,
    sale_id t_guid,
    delivery t_delivery,
    delivery_date date,
    description t_description,
    system_date timestamp without time zone DEFAULT now(),
    CONSTRAINT pk_delivery_notes PRIMARY KEY (delivery_id),
    CONSTRAINT fk_delivery_notes_user FOREIGN KEY (user_id) REFERENCES m_user (user_id),
    CONSTRAINT fk_delivery_notes_customer FOREIGN KEY (customer_id) REFERENCES m_customer (customer_id),
    CONSTRAINT fk_delivery_notes_invoice FOREIGN KEY (sale_id) REFERENCES t_product_sales (sale_id)
);

CREATE TABLE t_delivery_items (
    delivery_item_id t_guid NOT NULL,
    delivery_id t_guid,
    product_id t_guid,
    quantity t_quantity,
    description t_description,
    system_date timestamp without time zone DEFAULT now(),
    CONSTRAINT pk_delivery_items PRIMARY KEY (delivery_item_id),
    CONSTRAINT fk_delivery_items_delivery FOREIGN KEY (delivery_id) REFERENCES t_delivery_notes (delivery_id),
    CONSTRAINT fk_delivery_items_product FOREIGN KEY (product_id) REFERENCES m_product (product_id)
);




INSERT INTO m_menu (menu_id, name_menu, menu_title, parent_id, order_number, is_active, name_form, is_enabled)
VALUES ('e9a97a12-3ec0-4bab-a8f2-602efdedb481', 'mnuSalesDeliveryNotes', 'Delivery Notes', 'bd16c77f-24a0-44e9-b538-fa59e312ec68', '5', true, 'FrmListSalesDeliveryNotes', true);




COPY public.m_item_menu (item_menu_id, menu_id, grant_id, description) FROM stdin;
be7f30c2-7a17-4e8f-8dd3-00033d1324fc	e9a97a12-3ec0-4bab-a8f2-602efdedb481	0	Melihat Data
52e390a5-ec6b-4718-8c06-677136220372	e9a97a12-3ec0-4bab-a8f2-602efdedb481	1	Tambah Data
cac9969a-bb65-4627-a3dc-61613ad1ddb1	e9a97a12-3ec0-4bab-a8f2-602efdedb481	2	Edit Data
d8fad562-9f21-44a7-8af6-1828841eddbe	e9a97a12-3ec0-4bab-a8f2-602efdedb481	3	Hapus Data
\.



COPY public.m_role_privilege (role_id, menu_id, grant_id, is_grant) FROM stdin;
11dc1faf-2c66-4525-932d-a90e24da8987	e9a97a12-3ec0-4bab-a8f2-602efdedb481	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	e9a97a12-3ec0-4bab-a8f2-602efdedb481	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	e9a97a12-3ec0-4bab-a8f2-602efdedb481	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	e9a97a12-3ec0-4bab-a8f2-602efdedb481	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	e9a97a12-3ec0-4bab-a8f2-602efdedb481	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	e9a97a12-3ec0-4bab-a8f2-602efdedb481	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	e9a97a12-3ec0-4bab-a8f2-602efdedb481	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	e9a97a12-3ec0-4bab-a8f2-602efdedb481	3	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	e9a97a12-3ec0-4bab-a8f2-602efdedb481	0	f
\.



CREATE SEQUENCE t_delivery_notes_delivery_notes_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_delivery_notes_delivery_notes_id_seq OWNER TO postgres;


ALTER TABLE t_delivery_notes
ADD COLUMN valid_until date,
ADD COLUMN tax t_price,
ADD COLUMN discount t_price,
ADD COLUMN total_invoice t_price,
ADD COLUMN shipping_subdistrict t_address_long,
ADD COLUMN shipping_city t_address_long,
ADD COLUMN shipping_postal_code t_postal_code,
ADD COLUMN shipping_address t_address_long,
ADD COLUMN shipping_phone t_phone,
ADD COLUMN shipping_cost t_price,
ADD COLUMN shipping_to t_name,
ADD COLUMN shipping_village text,
ADD COLUMN from_label1 t_description,
ADD COLUMN from_label2 t_description,
ADD COLUMN from_label3 t_description,
ADD COLUMN from_label4 t_description,
ADD COLUMN to_label1 t_address_long,
ADD COLUMN to_label2 t_address_long,
ADD COLUMN to_label3 t_address_long,
ADD COLUMN to_label4 t_address_long,
ADD COLUMN courier t_description,
ADD COLUMN is_dropship boolean,
ADD COLUMN shipping_country t_address_long,
ADD COLUMN shipping_regency t_address_long,
ADD COLUMN machine_id t_guid,
ADD COLUMN payment_cash t_price,
ADD COLUMN payment_card t_price,
ADD COLUMN card_id t_guid,
ADD COLUMN card_number t_invoice,
ADD COLUMN dropshipper_id t_guid;

ALTER TABLE t_delivery_items
ADD COLUMN purchase_price t_price,
ADD COLUMN selling_price t_price,
ADD COLUMN discount t_quantity;
ADD COLUMN user_id t_guid;


ALTER TABLE t_delivery_notes
ADD CONSTRAINT fk_delivery_notes_machine_id
FOREIGN KEY (machine_id)
REFERENCES t_machine(machine_id);


ALTER TABLE t_delivery_notes
ADD CONSTRAINT fk_delivery_notes_card_id
FOREIGN KEY (card_id)
REFERENCES m_card(card_id);

ALTER TABLE t_delivery_notes
ADD CONSTRAINT fk_delivery_notes_dropshipper_id
FOREIGN KEY (dropshipper_id)
REFERENCES m_dropshipper(dropshipper_id);

ALTER TABLE t_delivery_items
ADD CONSTRAINT fk_delivery_items_user_id
FOREIGN KEY (user_id)
REFERENCES m_user(user_id);

ALTER TABLE t_delivery_items
ADD COLUMN return_quantity t_quantity;
