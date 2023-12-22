
CREATE DATABASE sparkposdb;

\c sparkposdb




--
-- PostgreSQL database dump
--


SET statement_timeout = 0;
SET lock_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET client_min_messages = warning;

--
-- Name: plpgsql; Type: EXTENSION; Schema: -; Owner: 
--



CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;


--
-- Name: EXTENSION plpgsql; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';


SET search_path = public, pg_catalog;

--
-- Name: t_address; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_address AS character varying(100);


ALTER DOMAIN t_address OWNER TO postgres;

--
-- Name: t_address_panjang; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_address_panjang AS character varying(250);


ALTER DOMAIN t_address_panjang OWNER TO postgres;

--
-- Name: t_bool; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_bool AS boolean DEFAULT true;


ALTER DOMAIN t_bool OWNER TO postgres;

--
-- Name: t_diskon; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_diskon AS numeric(5,2) DEFAULT 0.00;


ALTER DOMAIN t_diskon OWNER TO postgres;

--
-- Name: t_guid; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_guid AS character(36);


ALTER DOMAIN t_guid OWNER TO postgres;

--
-- Name: t_price; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_price AS numeric(15,2) DEFAULT 0.00;


ALTER DOMAIN t_price OWNER TO postgres;

--
-- Name: t_quantity; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_quantity AS numeric(10,2) DEFAULT 0.00;


ALTER DOMAIN t_quantity OWNER TO postgres;

--
-- Name: t_description; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_description AS character varying(100);


ALTER DOMAIN t_description OWNER TO postgres;

--
-- Name: t_postal_code; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_postal_code AS character varying(6);


ALTER DOMAIN t_postal_code OWNER TO postgres;

--
-- Name: t_product_code; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_product_code AS character varying(15);


ALTER DOMAIN t_product_code OWNER TO postgres;

--
-- Name: t_name; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_name AS character varying(50);


ALTER DOMAIN t_name OWNER TO postgres;

--
-- Name: t_name_panjang; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_name_panjang AS character varying(300);


ALTER DOMAIN t_name_panjang OWNER TO postgres;

--
-- Name: t_invoice; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_invoice AS character varying(20);


ALTER DOMAIN t_invoice OWNER TO postgres;

--
-- Name: t_password; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_password AS character(32);


ALTER DOMAIN t_password OWNER TO postgres;

--
-- Name: t_unit; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_unit AS character varying(20);


ALTER DOMAIN t_unit OWNER TO postgres;

--
-- Name: t_phone; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_phone AS character varying(20);


ALTER DOMAIN t_phone OWNER TO postgres;

--
-- Name: f_delete_payable_product_header(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION f_delete_payable_product_header() RETURNS trigger
    LANGUAGE plpgsql
    AS $$

DECLARE var_pembayaran_hutang_product_id t_guid;

DECLARE var_row_count integer;



BEGIN		    

    var_pembayaran_hutang_product_id := OLD.pay_purchase_id;

	

    var_row_count := (select count(*) from t_debt_payment_item

                      where pay_purchase_id = var_pembayaran_hutang_product_id);

                      

    IF var_row_count IS NULL THEN

        var_row_count := 0;  

    END IF;

	

	IF (var_row_count = 0) THEN

    	DELETE FROM t_product_payable_payment WHERE pay_purchase_id = var_pembayaran_hutang_product_id;

    END IF;

    

  	RETURN NULL;

END;

$$;


ALTER FUNCTION public.f_delete_payable_product_header() OWNER TO postgres;



CREATE FUNCTION f_reduce_product_stock() RETURNS trigger
    LANGUAGE plpgsql
    AS $$

DECLARE 

	var_product_id 				t_guid;

    

	var_stock_sekarang 			t_quantity; -- stock etalase	

    var_warehouse_stock_sekarang 	t_quantity; -- stock gudang

    

    var_quantity_lama 		t_quantity;

	var_quantity_baru 		t_quantity;

    

    var_return_quantity_lama 	t_quantity;

    var_return_quantity_baru 	t_quantity;



	var_price 				t_price;

    

    is_retur				t_bool;

    is_update_selling_price	t_bool;

    

BEGIN		

	is_retur := FALSE;

                

    IF TG_OP = 'INSERT' THEN

    	var_product_id := NEW.product_id;

        var_quantity_baru = NEW.quantity;        

        var_price = NEW.selling_price;



	ELSIF TG_OP = 'UPDATE' THEN      

    	var_product_id := NEW.product_id;

        var_quantity_baru = NEW.quantity;

        var_quantity_lama = OLD.quantity;

        var_price = NEW.selling_price;

        

        -- quantity retur

        var_return_quantity_baru = NEW.return_quantity;

        var_return_quantity_lama = OLD.return_quantity;        

    ELSE

    	var_product_id := OLD.product_id;

        var_quantity_lama = OLD.quantity;

        var_price = OLD.selling_price;

    END IF;        

            

    SELECT stock, warehouse_stock INTO var_stock_sekarang, var_warehouse_stock_sekarang

    FROM m_product WHERE product_id = var_product_id;

    

    IF var_stock_sekarang IS NULL THEN -- stock etalase

        var_stock_sekarang := 0;  

    END IF;

    

    IF var_warehouse_stock_sekarang IS NULL THEN -- stock gudang

        var_warehouse_stock_sekarang := 0;  

    END IF;

        

    IF TG_OP = 'UPDATE' THEN -- pengecekan retur

    	IF var_return_quantity_lama <> var_return_quantity_baru THEN

        	is_retur := TRUE;                    

            

            IF var_return_quantity_lama IS NULL THEN

                var_return_quantity_lama := 0;  

            END IF;

            

            IF var_return_quantity_baru IS NULL THEN

                var_return_quantity_baru := 0;  

            END IF;

                           

            var_warehouse_stock_sekarang = var_warehouse_stock_sekarang - var_return_quantity_lama + var_return_quantity_baru;

			            

            UPDATE m_product SET warehouse_stock = var_warehouse_stock_sekarang WHERE product_id = var_product_id;

        END IF;        

    END IF;

	

    IF (is_retur = FALSE) THEN -- bukan retur    	    	        

        IF TG_OP = 'INSERT' THEN

            var_warehouse_stock_sekarang := var_warehouse_stock_sekarang - var_quantity_baru;

            

            IF (var_warehouse_stock_sekarang < 0 AND var_stock_sekarang > 0) THEN -- stock gudang kurang, sisanya ambil dari stock etalase

            	var_stock_sekarang = var_stock_sekarang - abs(var_warehouse_stock_sekarang);

                

                IF (var_stock_sekarang >= 0) THEN

                	var_warehouse_stock_sekarang := 0; --stock gudang habis

                ELSE

                	var_warehouse_stock_sekarang := var_stock_sekarang;

                    var_stock_sekarang := 0;

                END IF;

            END IF;

                        

            

        ELSIF TG_OP = 'UPDATE' THEN      

            var_warehouse_stock_sekarang = var_warehouse_stock_sekarang + var_quantity_lama - var_quantity_baru;

        ELSE

            var_warehouse_stock_sekarang = var_warehouse_stock_sekarang + var_quantity_lama;

        END IF;

        

        IF TG_OP = 'INSERT' THEN   

            -- baca setting aplikasi

        	SELECT is_update_iselling_price_master_produk INTO is_update_selling_price
			
            FROM m_application_setting LIMIT 1;

            

            IF is_update_selling_price IS NULL THEN

				is_update_selling_price := TRUE;

            END IF;

    

        	IF (is_update_selling_price = TRUE) THEN -- update price jual di master produk

            	UPDATE m_product SET stock = var_stock_sekarang, warehouse_stock = var_warehouse_stock_sekarang, selling_price = var_price WHERE product_id = var_product_id;

            ELSE          

	            UPDATE m_product SET stock = var_stock_sekarang, warehouse_stock = var_warehouse_stock_sekarang WHERE product_id = var_product_id;        

			END IF;         

        ELSE        

            UPDATE m_product SET stock = var_stock_sekarang, warehouse_stock = var_warehouse_stock_sekarang WHERE product_id = var_product_id;        

        END IF;    

    END IF;	

            

    RETURN NULL;

END;

$$;


ALTER FUNCTION public.f_reduce_product_stock() OWNER TO postgres;






--
-- Name: f_delete_receivable_product_header(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION f_delete_receivable_product_header() RETURNS trigger
    LANGUAGE plpgsql
    AS $$

DECLARE var_pay_sale_id t_guid;

DECLARE var_row_count integer;



BEGIN		    

    var_pay_sale_id := OLD.pay_sale_id;

	

    var_row_count := (select count(*) from t_item_pembayaran_piutang_produk

                      where pay_sale_id = var_pay_sale_id);

                      

    IF var_row_count IS NULL THEN

        var_row_count := 0;  

    END IF;

	

	IF (var_row_count = 0) THEN

    	DELETE FROM t_product_receivable_paymentWHERE pay_sale_id = var_pay_sale_id;

    END IF;

    

  	RETURN NULL;

END;

$$;


ALTER FUNCTION public.f_delete_receivable_product_header() OWNER TO postgres;

--
-- Name: f_reduce_product_stock(); Type: FUNCTION; Schema: public; Owner: postgres
--


--
-- Name: f_adjust_product_stock_aiud(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION f_adjust_product_stock_aiud() RETURNS trigger
    LANGUAGE plpgsql
    AS $$

DECLARE

	var_product_id 						t_guid;

    

    var_stok_etalase					t_quantity;

    var_warehouse_stock						t_quantity;

    

    var_stock_addition_etalase			t_quantity;

    var_penambahan_warehouse_stock			t_quantity;

    

    var_stock_reduction_etalase		t_quantity;

    var_pengurangan_warehouse_stock			t_quantity;

    

    var_stock_addition_etalase_old		t_quantity;

    var_penambahan_warehouse_stock_old		t_quantity;

    

    var_stock_reduction_etalase_old	t_quantity;

    var_pengurangan_warehouse_stock_old		t_quantity;

    

BEGIN    

	IF TG_OP = 'INSERT' THEN

    	var_product_id := NEW.product_id;

        

        var_stock_addition_etalase = NEW.stock_addition;        

		var_penambahan_warehouse_stock = NEW.penambahan_warehouse_stock;        

        

        var_stock_reduction_etalase = NEW.stock_reduction;        

		var_pengurangan_warehouse_stock = NEW.pengurangan_warehouse_stock;        



	ELSIF TG_OP = 'UPDATE' THEN      

    	var_product_id := NEW.product_id;

        

        var_stock_addition_etalase = NEW.stock_addition;        

        var_stock_addition_etalase_old = OLD.stock_addition;        

        

		var_penambahan_warehouse_stock = NEW.penambahan_warehouse_stock;        

        var_penambahan_warehouse_stock_old = OLD.penambahan_warehouse_stock;        

        

        var_stock_reduction_etalase = NEW.stock_reduction;        

        var_stock_reduction_etalase_old = OLD.stock_reduction;        

        

		var_pengurangan_warehouse_stock = NEW.pengurangan_warehouse_stock;        

        var_pengurangan_warehouse_stock_old = OLD.pengurangan_warehouse_stock;        

               

    ELSE

    	var_product_id := OLD.product_id;

        

        var_stock_addition_etalase_old = OLD.stock_addition;                

        var_penambahan_warehouse_stock_old = OLD.penambahan_warehouse_stock;        

        

        var_stock_reduction_etalase_old = OLD.stock_reduction;                

        var_pengurangan_warehouse_stock_old = OLD.pengurangan_warehouse_stock; 

    END IF;

    

    SELECT stok, warehouse_stock INTO var_stok_etalase, var_warehouse_stock

    FROM m_product WHERE product_id = var_product_id;

    

    IF var_stok_etalase IS NULL THEN

        var_stok_etalase := 0;  

    END IF;

    

    IF var_warehouse_stock IS NULL THEN

        var_warehouse_stock := 0;  

    END IF;

    

    IF TG_OP = 'INSERT' THEN		         

        IF (var_stock_addition_etalase > 0 OR var_penambahan_warehouse_stock > 0) THEN

        	var_stok_etalase := var_stok_etalase + var_stock_addition_etalase;

            var_warehouse_stock := var_warehouse_stock + var_penambahan_warehouse_stock;                    	

        END IF;   	        

        

        IF (var_stock_reduction_etalase > 0 OR var_pengurangan_warehouse_stock > 0) THEN

        	var_stok_etalase := var_stok_etalase - var_stock_reduction_etalase;

            var_warehouse_stock := var_warehouse_stock - var_pengurangan_warehouse_stock;                    	

        END IF;        



	ELSIF TG_OP = 'UPDATE' THEN      		      	                     

        IF (var_stock_addition_etalase > 0 OR var_stock_addition_etalase_old > 0 OR var_penambahan_warehouse_stock > 0 OR var_penambahan_warehouse_stock_old > 0) THEN

        	var_stok_etalase := var_stok_etalase - var_stock_addition_etalase_old + var_stock_addition_etalase;

            var_warehouse_stock := var_warehouse_stock - var_penambahan_warehouse_stock_old + var_penambahan_warehouse_stock;                    	

        END IF;

        

        IF (var_stock_reduction_etalase > 0 OR var_stock_reduction_etalase_old > 0 OR var_pengurangan_warehouse_stock > 0 OR var_pengurangan_warehouse_stock_old > 0) THEN

			var_stok_etalase := var_stok_etalase + var_stock_reduction_etalase_old - var_stock_reduction_etalase;

            var_warehouse_stock := var_warehouse_stock + var_pengurangan_warehouse_stock_old - var_pengurangan_warehouse_stock;                    	

        END IF;                        

        

    ELSE    	        

        IF (var_stock_addition_etalase_old > 0 OR var_penambahan_warehouse_stock_old > 0) THEN

        	var_stok_etalase := var_stok_etalase - var_stock_addition_etalase_old;

            var_warehouse_stock := var_warehouse_stock - var_penambahan_warehouse_stock_old;

        END IF;

        

        IF (var_stock_reduction_etalase_old > 0 OR var_pengurangan_warehouse_stock_old > 0) THEN

        	var_stok_etalase := var_stok_etalase + var_stock_reduction_etalase_old;

            var_warehouse_stock := var_warehouse_stock + var_pengurangan_warehouse_stock_old;

        END IF;

    END IF;    

    

    UPDATE m_product SET stok = var_stok_etalase, warehouse_stock = var_warehouse_stock WHERE product_id = var_product_id;

            

    RETURN NULL;

END;

$$;


ALTER FUNCTION public.f_adjust_product_stock_aiud() OWNER TO postgres;

--
-- Name: f_add_product_stock(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION f_add_product_stock() RETURNS trigger
    LANGUAGE plpgsql
    AS $$

DECLARE 

	var_product_id 				t_guid;

	

    var_stock_awal				t_quantity;

	var_stock_akhir				t_quantity;

    var_stock_sekarang 			t_quantity; -- stock etalase

	var_warehouse_stock_sekarang 	t_quantity; -- stock gudang

    

    var_quantity_lama 			t_quantity;

	var_quantity_baru 			t_quantity;

	

    var_return_quantity_lama 		t_quantity;

    var_return_quantity_baru 		t_quantity;

    

	var_price 					t_price; -- price beli di item beli

    var_hpp_awal				t_price; -- price beli di master produk

    var_hpp_akhir				t_price;

	var_starting_balance				t_price; -- (stock etalasi + stock gudang) * price beli di master produk 

    var_saldo_pembelian			t_price;

    

    is_retur					t_bool;

  

BEGIN		   

	is_retur := FALSE;	

    	     

    IF TG_OP = 'INSERT' THEN

    	var_product_id := NEW.product_id;

        var_quantity_baru = NEW.quantity;

        var_price = NEW.price;

        

	ELSIF TG_OP = 'UPDATE' THEN      

    	var_product_id := NEW.product_id;

        var_quantity_baru = NEW.quantity;

        var_quantity_lama = OLD.quantity;

        var_price = NEW.price;

        

        -- quantity retur

        var_return_quantity_baru = NEW.return_quantity;

        var_return_quantity_lama = OLD.return_quantity;                

    ELSE

    	var_product_id := OLD.product_id;

        var_quantity_baru = 0;

        var_quantity_lama = OLD.quantity;

        var_price = OLD.price;

    END IF;        

    

    SELECT	stock, warehouse_stock, purchase_price INTO var_stock_sekarang, var_warehouse_stock_sekarang, var_hpp_awal

    FROM m_product WHERE product_id = var_product_id;

    

    IF var_stock_sekarang IS NULL THEN

    	var_stock_sekarang := 0;  

	END IF;

    

    IF var_warehouse_stock_sekarang IS NULL THEN

    	var_warehouse_stock_sekarang := 0;  

	END IF;

    

    IF var_hpp_awal IS NULL THEN

    	var_hpp_awal := 0;  

	END IF;        

    

    var_stock_akhir := 1;

    var_stock_awal := var_stock_sekarang + var_warehouse_stock_sekarang;    

    var_starting_balance := var_stock_awal * var_hpp_awal;

    

    IF TG_OP = 'UPDATE' THEN -- pengecekan retur

    	IF var_return_quantity_lama <> var_return_quantity_baru THEN

        	is_retur := TRUE;                    

            

            IF var_return_quantity_lama IS NULL THEN

                var_return_quantity_lama := 0;  

            END IF;

            

            IF var_return_quantity_baru IS NULL THEN

                var_return_quantity_baru := 0;  

            END IF;

			          

            var_saldo_pembelian := ABS(var_return_quantity_lama - var_return_quantity_baru) * var_price;  

            

            var_stock_akhir := (var_stock_awal - ABS(var_return_quantity_lama - var_return_quantity_baru));

            IF (var_stock_akhir = 0) THEN

            	var_stock_akhir := 1;

            END IF;

            

            var_hpp_akhir := (var_starting_balance - var_saldo_pembelian) / var_stock_akhir;

                                       

            var_warehouse_stock_sekarang = var_warehouse_stock_sekarang + var_return_quantity_lama - var_return_quantity_baru;			            

                          			            

            UPDATE m_product SET warehouse_stock = var_warehouse_stock_sekarang, purchase_price = ROUND(var_hpp_akhir, 0) WHERE product_id = var_product_id;

        END IF;        

    END IF;

    

    IF (is_retur = FALSE) THEN -- bukan retur    	        	                                    

      IF TG_OP = 'INSERT' THEN                        

          var_saldo_pembelian := var_quantity_baru * var_price;          

          

          var_stock_akhir := (var_stock_awal + var_quantity_baru);

          

          IF (var_stock_akhir = 0) THEN

              var_stock_akhir := 1;

          END IF;

          

		  var_hpp_akhir := (var_starting_balance + var_saldo_pembelian) / var_stock_akhir;                                         

          

          var_warehouse_stock_sekarang = var_warehouse_stock_sekarang + var_quantity_baru;

          

      ELSIF TG_OP = 'UPDATE' THEN             

          var_saldo_pembelian := ABS(var_quantity_baru - var_quantity_lama) * var_price;

          var_hpp_akhir := var_hpp_awal;

          

      	  IF (var_quantity_baru > var_quantity_lama) THEN      

          	

          	 var_stock_akhir := (var_stock_awal + var_quantity_baru - var_quantity_lama);

             IF (var_stock_akhir = 0) THEN

             	var_stock_akhir := 1;

             END IF;



          	 var_hpp_akhir := (var_starting_balance + var_saldo_pembelian) / var_stock_akhir;                     

             

          ELSEIF (var_quantity_baru < var_quantity_lama) THEN          	 

          	 

          	 var_stock_akhir := (var_stock_awal - ABS(var_quantity_baru - var_quantity_lama));

             IF (var_stock_akhir = 0) THEN

             	var_stock_akhir := 1;

             END IF;

             

          	 var_hpp_akhir := (var_starting_balance - var_saldo_pembelian) / var_stock_akhir;                                   

          END IF;          

                                        

          var_warehouse_stock_sekarang = var_warehouse_stock_sekarang - var_quantity_lama + var_quantity_baru;

      ELSE -- DELETE

          var_saldo_pembelian := var_quantity_lama * var_price;

          

          var_stock_akhir := (var_stock_awal - var_quantity_lama);

          IF (var_stock_akhir = 0) THEN

             var_stock_akhir := 1;

          END IF;

             

          var_hpp_akhir := (var_starting_balance - var_saldo_pembelian) / var_stock_akhir;                     

                    

          var_warehouse_stock_sekarang = var_warehouse_stock_sekarang - var_quantity_lama;          

      END IF;          

      

	  UPDATE m_product SET warehouse_stock = var_warehouse_stock_sekarang, purchase_price = ROUND(var_hpp_akhir, 0) WHERE product_id = var_product_id;              

    END IF;    	

            

    RETURN NULL;

END;

$$;


ALTER FUNCTION public.f_add_product_stock() OWNER TO postgres;
--
-- Name: f_update_purchase_return_quantity(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION f_update_purchase_return_quantity() RETURNS trigger
    LANGUAGE plpgsql
    AS $$

DECLARE

	var_purchase_item_id 		t_guid;

    var_return_quantity		t_quantity;

        

BEGIN	

      

	IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN

    	var_purchase_item_id := NEW.purchase_item_id;    

        var_return_quantity := NEW.return_quantity;

    ELSE

    	var_purchase_item_id := OLD.purchase_item_id;

        var_return_quantity := 0;

    END IF;

    

    IF var_return_quantity IS NULL THEN

        var_return_quantity := 0;  

    END IF; 

            

    UPDATE t_purchase_order_item SET return_quantity = var_return_quantity 

    WHERE purchase_item_id = var_purchase_item_id;

	

	RETURN NULL;

END;

$$;


ALTER FUNCTION public.f_update_purchase_return_quantity() OWNER TO postgres;

--
-- Name: f_update_return_quantity_sales(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION f_update_return_quantity_sales() RETURNS trigger
    LANGUAGE plpgsql
    AS $$

DECLARE

	var_sale_item_id 		t_guid;

    var_return_quantity		t_quantity;

        

BEGIN	

      

	IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN

    	var_sale_item_id := NEW.sale_item_id;    

        var_return_quantity := NEW.return_quantity;

    ELSE

    	var_sale_item_id := OLD.sale_item_id;

        var_return_quantity := 0;

    END IF;

    

    IF var_return_quantity IS NULL THEN

        var_return_quantity := 0;  

    END IF; 

            

    UPDATE t_sales_order_item SET return_quantity = var_return_quantity 

    WHERE sale_item_id = var_sale_item_id;

	

	RETURN NULL;

END;

$$;


ALTER FUNCTION public.f_update_return_quantity_sales() OWNER TO postgres;

--
-- Name: f_update_purchase_payment(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION f_update_purchase_payment() RETURNS trigger
    LANGUAGE plpgsql
    AS $$

DECLARE var_purchase_id t_guid;

DECLARE var_pelunasan_nota t_price;

  

BEGIN

	IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN

    	var_purchase_id := NEW.purchase_id;    

    ELSE

    	var_purchase_id := OLD.purchase_id;

    END IF;

	        

    var_pelunasan_nota := (SELECT SUM(amount) FROM t_debt_payment_item 

    			 	       WHERE purchase_id = var_purchase_id);

	

    IF var_pelunasan_nota IS NULL THEN

    	var_pelunasan_nota := 0;  

	END IF;

    

    UPDATE t_purchase_product SET total_payment = var_pelunasan_nota 

    WHERE purchase_id = var_purchase_id;                        

    

    RETURN NULL;

END;

$$;


ALTER FUNCTION public.f_update_purchase_payment() OWNER TO postgres;

--
-- Name: f_update_sales_payment(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION f_update_sales_payment() RETURNS trigger
    LANGUAGE plpgsql
    AS $$

DECLARE var_sale_id t_guid;

DECLARE var_pelunasan_nota t_price;

  

BEGIN

	IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN

    	var_sale_id := NEW.sale_id;    

    ELSE

    	var_sale_id := OLD.sale_id;

    END IF;

	        

    var_pelunasan_nota := (SELECT SUM(amount) FROM t_credit_payment_item

    			 	       WHERE sale_id = var_sale_id);	

    IF var_pelunasan_nota IS NULL THEN

    	var_pelunasan_nota := 0;  

	END IF;

    

    UPDATE t_product_sales SET total_payment = var_pelunasan_nota 

    WHERE sale_id = var_sale_id;                        

    

    RETURN NULL;

END;

$$;


ALTER FUNCTION public.f_update_sales_payment() OWNER TO postgres;

--
-- Name: f_update_loan_payment(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION f_update_loan_payment() RETURNS trigger
    LANGUAGE plpgsql
    AS $$

DECLARE 

	var_loan_id				t_guid;   

    var_gaji_employee_id		t_guid; 

	var_total_settlement_loan 	t_price;



BEGIN

	IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN

    	var_loan_id := NEW.loan_id;    

        var_gaji_employee_id := NEW.gaji_employee_id;

    ELSE

    	var_loan_id := OLD.loan_id;

        var_gaji_employee_id := OLD.gaji_employee_id;

    END IF;

	        

    -- pelunasan loan

    var_total_settlement_loan := (SELECT SUM(amount) FROM t_pembayaran_loan 

    							   WHERE loan_id = var_loan_id);	    

	

    IF var_total_settlement_loan IS NULL THEN

    	var_total_settlement_loan := 0;  

	END IF;        

    

    -- loan

    UPDATE t_loan SET total_payment = var_total_settlement_loan 

    WHERE loan_id = var_loan_id;    

    

    -- hitung total pelunasan yang dibayar pake gaji (deductions gaji untuk loan) 

    IF NOT (var_gaji_employee_id IS NULL) THEN

    	-- pelunasan loan

    	var_total_settlement_loan := (SELECT SUM(amount) FROM t_pembayaran_loan 

    							   	   WHERE gaji_employee_id = var_gaji_employee_id);	    

	

	    IF var_total_settlement_loan IS NULL THEN

    		var_total_settlement_loan := 0;  

		END IF;

    	

        UPDATE t_employee_salary SET loan = var_total_settlement_loan 

	    WHERE gaji_employee_id = var_gaji_employee_id;

	END IF;

    

    RETURN NULL;

END;

$$;


ALTER FUNCTION public.f_update_loan_payment() OWNER TO postgres;

--
-- Name: f_update_total_purchase(); Type: FUNCTION; Schema: public; Owner: postgres
--
CREATE FUNCTION f_update_total_purchase() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
DECLARE 
	var_purchase_id 		t_guid;
    var_return_purchase_id		t_guid;
    var_diskon				t_price;
    var_ppn					t_price;
	var_total_nota 			t_price;
  	var_return_quantity_lama 	t_quantity;
    var_return_quantity_baru 	t_quantity;
    var_date_tempo		DATE;  
    is_retur				t_bool;
    
BEGIN
	is_retur := FALSE;
    
	IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN
    	var_purchase_id := NEW.purchase_id;    
    ELSE
    	var_purchase_id := OLD.purchase_id;
    END IF;	 
                       
    var_total_nota := (SELECT SUM((quantity - return_quantity) * (price - (discount / 100 * price))) 
    				   FROM t_purchase_order_item
					   WHERE purchase_id = var_purchase_id);
	
    IF var_total_nota IS NULL THEN
    	var_total_nota := 0;  
	END IF;              
    --
    SELECT tax, discount INTO var_ppn, var_diskon
    FROM t_purchase_product WHERE purchase_id = var_purchase_id;            
    
    IF var_ppn IS NULL THEN
    	var_ppn := 0;  
	END IF;
    
    IF var_diskon IS NULL THEN
    	var_diskon := 0;  
	END IF;           

	IF TG_OP = 'UPDATE' THEN -- pengecekan retur
    	-- quantity retur
        var_return_quantity_baru = NEW.return_quantity;
        var_return_quantity_lama = OLD.return_quantity;        
        
    	IF var_return_quantity_lama <> var_return_quantity_baru THEN
			is_retur := TRUE;
			
            var_return_purchase_id := (SELECT purchase_return_id FROM t_purchase_return WHERE purchase_id = var_purchase_id LIMIT 1);
			var_date_tempo := (SELECT date_tempo FROM t_purchase_product WHERE purchase_id = var_purchase_id);                                    
            
            IF var_date_tempo IS NULL THEN -- invoice tunai            	
                UPDATE t_purchase_product SET total_invoice = ROUND(var_total_nota, 0), purchase_return_id = var_return_purchase_id 
                WHERE purchase_id = var_purchase_id;                
                
                UPDATE t_debt_payment_item SET amount = ROUND(var_total_nota, 0) - var_diskon + var_ppn
                WHERE purchase_id = var_purchase_id;
            ELSE
            	UPDATE t_purchase_product SET total_invoice = ROUND(var_total_nota, 0), purchase_return_id = var_return_purchase_id 
                WHERE purchase_id = var_purchase_id;
            END IF;        
        END IF;        
    END IF;
    
    IF (is_retur = FALSE) THEN -- bukan retur    	
	    UPDATE t_purchase_product SET total_invoice = ROUND(var_total_nota, 0) 
        WHERE purchase_id = var_purchase_id;
    END IF;
    
    RETURN NULL;
END;
$$;


ALTER FUNCTION public.f_update_total_purchase() OWNER TO postgres;



--
-- Name: f_update_supplier_total_debt(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION f_update_supplier_total_debt() RETURNS trigger
    LANGUAGE plpgsql
    AS $$

DECLARE 	

    var_supplier_id 				t_guid;  

    var_supplier_id_old				t_guid;

    

    var_total_debt_produk			t_price;

  	var_total_settlement_produk		t_price;

    

BEGIN	    

	IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN

    	var_supplier_id := NEW.supplier_id;        

    ELSE

    	var_supplier_id := OLD.supplier_id;

        var_supplier_id_old := OLD.supplier_id;

    END IF;

	    	           	                

    -- hitung total hutang dan pelunasan pembelian produk

    SELECT SUM(total_invoice - discount + tax) AS total_debt, SUM(total_payment) AS total_payment

    INTO var_total_debt_produk, var_total_settlement_produk

    FROM t_purchase_product 

    WHERE date_tempo IS NOT NULL AND supplier_id = var_supplier_id;



    IF var_total_debt_produk IS NULL THEN

        var_total_debt_produk := 0;  

    END IF; 

        

    IF var_total_settlement_produk IS NULL THEN

        var_total_settlement_produk := 0;  

    END IF;

                

    UPDATE m_supplier SET total_debt = var_total_debt_produk, total_debt_payment = var_total_settlement_produk 

    WHERE supplier_id = var_supplier_id;       

    

    IF TG_OP = 'UPDATE' THEN

    	var_supplier_id_old := OLD.supplier_id;

        

    	IF var_supplier_id <> var_supplier_id_old THEN

            -- hitung total hutang dan pelunasan pembelian produk

            SELECT SUM(total_invoice - discount + tax) AS total_debt, SUM(total_payment) AS total_payment

            INTO var_total_debt_produk, var_total_settlement_produk

            FROM t_purchase_product             

            WHERE date_tempo IS NOT NULL AND supplier_id = var_supplier_id_old;



            IF var_total_debt_produk IS NULL THEN

                var_total_debt_produk := 0;  

            END IF; 

                

            IF var_total_settlement_produk IS NULL THEN

                var_total_settlement_produk := 0;  

            END IF;

                

            UPDATE m_supplier SET total_debt = var_total_debt_produk, total_debt_payment = var_total_settlement_produk 

            WHERE supplier_id = var_supplier_id_old;                                    

    		                        

            UPDATE t_product_payable_payment SET supplier_id = var_supplier_id

            WHERE pay_purchase_id IN (SELECT pay_purchase_id FROM t_debt_payment_item WHERE purchase_id = NEW.purchase_id);                

            

        END IF;

    END IF;            

    

    RETURN NULL;

END;

$$;


ALTER FUNCTION public.f_update_supplier_total_debt() OWNER TO postgres;

--
-- Name: f_update_total_sales(); Type: FUNCTION; Schema: public; Owner: postgres
--
CREATE FUNCTION f_update_total_sales() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
DECLARE 
	var_sale_id 			t_guid;
    var_return_sale_id		t_guid;
    var_diskon				t_price;
    var_ppn					t_price;
	var_shipping_cost		t_price;
	var_total_nota 			t_price;        
    var_return_quantity_lama 	t_quantity;
    var_return_quantity_baru 	t_quantity;
    var_due_date		DATE;
    is_retur				t_bool;
    
BEGIN
	is_retur := FALSE;
    
	IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN
    	var_sale_id := NEW.sale_id;    
    ELSE
    	var_sale_id := OLD.sale_id;        
    END IF;	    	          

    var_total_nota := (SELECT SUM((quantity - return_quantity) * (selling_price - (discount / 100 * selling_price))) 
    				   FROM t_sales_order_item
					   WHERE sale_id = var_sale_id);	    
	IF var_total_nota IS NULL THEN
    	var_total_nota := 0;  
	END IF;     
    
    var_total_nota := ROUND(var_total_nota, 0);      
    
	SELECT tax, shipping_cost, discount INTO var_ppn, var_shipping_cost, var_diskon
    FROM t_product_sales WHERE sale_id = var_sale_id;            
    
    IF var_ppn IS NULL THEN
    	var_ppn := 0;  
	END IF;
    
	IF var_shipping_cost IS NULL THEN
    	var_shipping_cost := 0;  
	END IF;
	
    IF var_diskon IS NULL THEN
    	var_diskon := 0;  
	END IF;
    
    IF TG_OP = 'UPDATE' THEN -- pengecekan retur
    	-- quantity retur
        var_return_quantity_baru = NEW.return_quantity;
        var_return_quantity_lama = OLD.return_quantity;        
        
    	IF var_return_quantity_lama <> var_return_quantity_baru THEN
			is_retur := TRUE;
			
            var_return_sale_id := (SELECT return_sale_id FROM t_sales_return WHERE sale_id = var_sale_id LIMIT 1);
            var_due_date := (SELECT due_date FROM t_product_sales WHERE sale_id = var_sale_id);                                                
            
            IF var_due_date IS NULL THEN -- invoice tunai            	
                UPDATE t_product_sales SET total_invoice = ROUND(var_total_nota, 0), return_sale_id = var_return_sale_id 
                WHERE sale_id = var_sale_id;                
                
                UPDATE t_credit_payment_item SET amount = ROUND(var_total_nota, 0) - var_diskon + var_ppn + var_shipping_cost
                WHERE sale_id = var_sale_id;
            ELSE
            	UPDATE t_product_sales SET total_invoice = ROUND(var_total_nota, 0), return_sale_id = var_return_sale_id 
                WHERE sale_id = var_sale_id;
            END IF;        
        END IF;        
    END IF;              

	IF (is_retur = FALSE) THEN -- bukan retur    	
    	UPDATE t_product_sales SET total_invoice = ROUND(var_total_nota, 0), return_sale_id = NULL 
        WHERE sale_id = var_sale_id;
    END IF;
    
    RETURN NULL;
END;
$$;


ALTER FUNCTION public.f_update_total_sales() OWNER TO postgres;
--
-- Name: f_update_employee_total_loan(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION f_update_employee_total_loan() RETURNS trigger
    LANGUAGE plpgsql
    AS $$

DECLARE 

    var_employee_id		t_guid;

    

	var_total_loan 	t_price;

  	var_total_settlement	t_price;

    

BEGIN

	IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN

    	var_employee_id := NEW.employee_id;    

    ELSE

    	var_employee_id := OLD.employee_id;

    END IF;	            

	

    SELECT SUM(amount), SUM(total_payment)

    INTO var_total_loan, var_total_settlement

	FROM t_loan WHERE employee_id = var_employee_id;

        

    IF var_total_loan IS NULL THEN

    	var_total_loan := 0;  

	END IF;

    

    IF var_total_settlement IS NULL THEN

    	var_total_settlement := 0;  

	END IF;

        

    UPDATE m_employee SET total_loan = var_total_loan, total_loan_payment = var_total_settlement 

    WHERE employee_id = var_employee_id;

    

    RETURN NULL;

END;

$$;


ALTER FUNCTION public.f_update_employee_total_loan() OWNER TO postgres;

--
-- Name: f_update_total_expenses(); Type: FUNCTION; Schema: public; Owner: postgres
--




CREATE FUNCTION f_update_total_expenses() RETURNS trigger
    LANGUAGE plpgsql
    AS $$

DECLARE v_expense_id t_guid;

DECLARE v_total t_price;

  

BEGIN

	IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN

    	v_expense_id := NEW.expense_id;    

    ELSE

    	v_expense_id := OLD.expense_id;

    END IF;

        

    v_total := (SELECT SUM(quantity * price) FROM t_expense_item

			    WHERE expense_id = v_expense_id);

	

    IF v_total IS NULL THEN

    	v_total := 0;  

	END IF;

    

    UPDATE t_expence SET total = v_total WHERE expense_id = v_expense_id;                        

    RETURN NULL;

END;

$$;


ALTER FUNCTION public.f_update_total_expenses() OWNER TO postgres;



--
-- Name: f_update_customer_total_credit(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION f_update_customer_total_credit() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
DECLARE 	
    var_total_credit	t_price;
    var_total_settlement	t_price;
  	var_customer_id		t_guid;
    var_customer_id_old	t_guid;
    
BEGIN
	IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN
    	var_customer_id := NEW.customer_id;        
    ELSE
    	var_customer_id := OLD.customer_id;
        var_customer_id_old = OLD.customer_id;
    END IF;
	    	            
	SELECT SUM(total_invoice - discount + shipping_cost + tax), SUM(total_payment)
    INTO var_total_credit, var_total_settlement
	FROM t_product_sales    
    WHERE due_date IS NOT NULL AND customer_id = var_customer_id;
    
    IF var_total_credit IS NULL THEN
        var_total_credit := 0;  
    END IF;
    
    IF var_total_settlement IS NULL THEN
        var_total_settlement := 0;  
    END IF;
        
    UPDATE m_customer SET total_credit = var_total_credit, total_receivable_payment = var_total_settlement
	WHERE customer_id = var_customer_id;        
    
    IF TG_OP = 'UPDATE' THEN
		var_customer_id_old = OLD.customer_id;
		
        IF var_customer_id <> var_customer_id_old THEN
        	SELECT SUM(total_invoice - discount + shipping_cost + tax), SUM(total_payment)
            INTO var_total_credit, var_total_settlement
            FROM t_product_sales            
            WHERE due_date IS NOT NULL AND customer_id = var_customer_id_old;
            
            IF var_total_credit IS NULL THEN
                var_total_credit := 0;  
            END IF;
            
            IF var_total_settlement IS NULL THEN
                var_total_settlement := 0;  
            END IF;                           
			
			UPDATE m_customer SET total_credit = var_total_credit, total_receivable_payment = var_total_settlement
            WHERE customer_id = var_customer_id_old;
            
            UPDATE t_product_receivable_payment SET customer_id = var_customer_id 
            WHERE pay_sale_id IN (SELECT pay_sale_id FROM t_credit_payment_item WHERE sale_id = NEW.sale_id);
        END IF;
    END IF;
    
    RETURN NULL;
END;
$$;



ALTER FUNCTION public.f_update_customer_total_credit() OWNER TO postgres;





--
-- Name: f_update_purchase_return_total_aiud(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION f_update_purchase_return_total_aiud() RETURNS trigger
    LANGUAGE plpgsql
    AS $$

DECLARE

	var_purchase_return_id	t_guid;

	var_total_nota 		t_price;

    

BEGIN    

    IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN

    	var_purchase_return_id := NEW.purchase_return_id;    

    ELSE

    	var_purchase_return_id := OLD.purchase_return_id;

    END IF;

        

    var_total_nota := (SELECT SUM(return_quantity * price) 

    				   FROM t_purchase_return_item

					   WHERE purchase_return_id = var_purchase_return_id);

                           

    IF var_total_nota IS NULL THEN

    	var_total_nota := 0;  

	END IF;                         

    

    UPDATE t_purchase_return SET total_invoice = var_total_nota 

    WHERE purchase_return_id = var_purchase_return_id;

    

    RETURN NULL;

END;

$$;


ALTER FUNCTION public.f_update_purchase_return_total_aiud() OWNER TO postgres;


--
-- Name: f_update_sales_return_total_aiud(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION f_update_sales_return_total_aiud() RETURNS trigger
    LANGUAGE plpgsql
    AS $$

DECLARE

	var_return_sale_id	t_guid;

	var_total_nota 		t_price;

    

BEGIN    

    IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN

    	var_return_sale_id := NEW.return_sale_id;    

    ELSE

    	var_return_sale_id := OLD.return_sale_id;

    END IF;

        

    var_total_nota := (SELECT SUM(return_quantity * selling_price) 

    				   FROM t_sales_return_item

					   WHERE return_sale_id = var_return_sale_id);

                           

    IF var_total_nota IS NULL THEN

    	var_total_nota := 0;  

	END IF;                         

    

    UPDATE t_sales_return SET total_invoice = var_total_nota 

    WHERE return_sale_id = var_return_sale_id;

    

    RETURN NULL;

END;

$$;


ALTER FUNCTION public.f_update_sales_return_total_aiud() OWNER TO postgres;
--
-- Name: fn_log_last_update(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION fn_log_last_update() RETURNS trigger
    LANGUAGE plpgsql
    AS $$

DECLARE 	

    var_product_id		t_guid;

    var_iselling_price		t_price;

    var_iselling_price_old	t_price;

    

BEGIN

	IF TG_OP = 'UPDATE' THEN

    	var_product_id := NEW.product_id;

        var_iselling_price := NEW.selling_price;        

        var_iselling_price_old := OLD.selling_price;    	

    END IF;

    

    IF var_iselling_price IS NULL THEN

    	var_iselling_price := 0;

    END IF;

    	   

    IF var_iselling_price_old IS NULL THEN

    	var_iselling_price_old := 0;

    END IF;

    

    IF var_iselling_price_old <> var_iselling_price THEN

    	UPDATE m_product SET last_update = now() WHERE product_id = var_product_id;

    END IF;

    

    RETURN NULL;

END;

$$;


ALTER FUNCTION public.fn_log_last_update() OWNER TO postgres;

SET default_tablespace = '';

SET default_with_oids = false;

--
-- Name: m_adjustment_reason; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_adjustment_reason(
    stock_adjustment_reason_id t_guid NOT NULL,
    reason t_description
);


ALTER TABLE m_adjustment_reason OWNER TO postgres;

--
-- Name: m_customer; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_customer (
    customer_id t_guid NOT NULL,
    name_customer t_name,
    address t_address_panjang,
    contact t_name,
    phone t_phone,
    credit_limit t_price,
    total_credit t_price,
    total_receivable_payment t_price,
    subdistrict t_address,
    city t_address,
    postal_code t_postal_code,
    discount t_quantity,
    country t_address,
    regency t_address,
    province_id character(2),
    regency_id character(4),
    subdistrict_id character(7),
	village t_address
);


ALTER TABLE m_customer OWNER TO postgres;

--
-- Name: m_database_version; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_database_version (
    version_number integer NOT NULL
);


ALTER TABLE m_database_version OWNER TO postgres;

--
-- Name: m_dropshipper; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_dropshipper (
    dropshipper_id t_guid NOT NULL,
    name_dropshipper t_name,
    address t_address,
    contact t_name,
    phone t_phone
);


ALTER TABLE m_dropshipper OWNER TO postgres;

--
-- Name: m_mini_pos_invoice_footer; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_mini_pos_invoice_footer (
    footer_invoice_id t_guid NOT NULL,
    description t_description,
    order_number integer,
    is_active t_bool
);


ALTER TABLE m_mini_pos_invoice_footer OWNER TO postgres;

--
-- Name: m_category; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_category (
    category_id t_guid NOT NULL,
    name_category t_name,
    discount t_quantity,
    profit_percentage t_quantity
);


ALTER TABLE m_category OWNER TO postgres;

--
-- Name: m_wholesale_price; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_wholesale_price (
    wholesale_price_id t_guid NOT NULL,
    product_id t_guid,
    retail_price integer,
    wholesale_price t_price,
    minimum_quantity t_quantity,
    discount t_quantity
);


ALTER TABLE m_wholesale_price OWNER TO postgres;

--
-- Name: m_invoice_header; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_invoice_header (
    header_invoice_id t_guid NOT NULL,
    description t_description,
    order_number integer,
    is_active t_bool
);


ALTER TABLE m_invoice_header OWNER TO postgres;

--
-- Name: m_invoice_header_mini_pos; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_invoice_header_mini_pos (
    header_invoice_id t_guid NOT NULL,
    description t_description,
    order_number integer,
    is_active t_bool
);


ALTER TABLE m_invoice_header_mini_pos OWNER TO postgres;

--
-- Name: m_item_menu; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_item_menu (
    item_menu_id t_guid NOT NULL,
    menu_id t_guid,
    grant_id integer,
    description t_description
);


ALTER TABLE m_item_menu OWNER TO postgres;

--
-- Name: COLUMN m_item_menu.grant_id; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN m_item_menu.grant_id IS 'Mereference ke tabel m_role_privilege';


--
-- Name: m_job_titles; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_job_titles (
    job_titles_id t_guid NOT NULL,
    name_job_titles t_name,
    description t_description
);


ALTER TABLE m_job_titles OWNER TO postgres;

--
-- Name: m_expense_type; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_expense_type (
    expense_type_id t_guid NOT NULL,
    name_expense_type t_description
);


ALTER TABLE m_expense_type OWNER TO postgres;

--
-- Name: m_regency; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_regency (
    regency_id integer NOT NULL,
    province_id integer,
    type character varying(15),
    name_regency t_description,
    postal_code t_postal_code
);


ALTER TABLE m_regency OWNER TO postgres;

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
    stock t_quantity,
    purchase_price t_price,
    selling_price t_price,
    product_code t_product_code,
    category_id t_guid,
    minimal_stock t_quantity DEFAULT 0,
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
    purchase_id t_guid NOT NULL,
    user_id t_guid,
    supplier_id t_guid,
    purchase_return_id t_guid,
    invoice t_invoice,
    date date,
    date_tempo date,
    tax t_price,
    discount t_price,
    total_invoice t_price,
    total_payment t_price,
    description t_description,
    system_date timestamp without time zone DEFAULT now(),
	due_date date
);


ALTER TABLE t_purchase_product OWNER TO postgres;

--
-- Name: t_purchase_product_beli_product_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--


CREATE SEQUENCE t_purchase_product_purchase_product_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_purchase_product_purchase_product_id_seq OWNER TO postgres;

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
    invoice t_invoice
);


ALTER TABLE t_employee_salary OWNER TO postgres;

--
-- Name: t_employee_salary_gaji_employee_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--


CREATE SEQUENCE t_employee_salary_employee_salary_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_employee_salary_employee_salary_id_seq OWNER TO postgres;


--
-- Name: t_purchase_order_item; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_purchase_order_item (
     purchase_item_id t_guid NOT NULL,
    purchase_id t_guid,
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
    sale_item_id t_guid NOT NULL,
    sale_id t_guid,
    user_id t_guid,
    product_id t_guid,
    purchase_price t_price,
    selling_price t_price,
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
    pay_purchase_item_id t_guid NOT NULL,
    pay_purchase_id t_guid,
    purchase_id t_guid,
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
    return_purchase_item_id t_guid NOT NULL,
    purchase_return_id t_guid,
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
    return_sale_item_id t_guid NOT NULL,
    return_sale_id t_guid,
    user_id t_guid,
    product_id t_guid,
    selling_price t_price,
    quantity t_quantity,
    system_date timestamp without time zone DEFAULT now(),
    return_quantity t_quantity,
    sale_item_id t_guid
);


ALTER TABLE t_sales_return_item OWNER TO postgres;

--
-- Name: t_product_sales; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_product_sales(
    sale_id t_guid NOT NULL,
    user_id t_guid,
    customer_id t_guid,
    invoice t_invoice,
    date date,
    due_date date,
    tax t_price,
    discount t_price,
    total_invoice t_price,
    total_payment t_price,
    description t_description,
    system_date timestamp without time zone DEFAULT now(),
    return_sale_id t_guid,
    shift_id t_guid,
    is_sdac t_bool,
    shipping_subdistrict t_address_panjang,
    shipping_city t_address_panjang,
    shipping_postal_code t_postal_code,
    shipping_kepada t_name,
    shipping_address t_address_panjang,
    shipping_phone t_phone,
    shipping_cost t_price,
	shipping_to text,
	shipping_village text,
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
    card_number t_invoice,
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


CREATE SEQUENCE t_product_sales_product_sales_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_product_sales_product_sales_id_seq OWNER TO postgres;

--
-- Name: t_loan; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_loan (
    loan_id t_guid NOT NULL,
    employee_id t_guid,
    user_id t_guid,
    invoice t_invoice,
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
    log_id bigint DEFAULT nextval(('public.t_logs_logs_id_seq'::text)::regclass) NOT NULL,
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




CREATE SEQUENCE t_logs_logs_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_logs_logs_id_seq OWNER TO postgres;

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
    pay_purchase_id t_guid NOT NULL,
    supplier_id t_guid,
    user_id t_guid,
    date date,
    description t_description,
    system_date timestamp without time zone DEFAULT now(),
    invoice t_invoice,
    is_cash t_bool
);


ALTER TABLE t_product_payable_payment OWNER TO postgres;

--
-- Name: t_product_payable_payment_pembayaran_hutang_product_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--


CREATE SEQUENCE t_product_payable_payment_product_payable_payment_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_product_payable_payment_product_payable_payment_id_seq OWNER TO postgres;

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
    invoice t_invoice,
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
    invoice t_invoice,
    is_cash t_bool
);


ALTER TABLE t_product_receivable_payment OWNER TO postgres;

--
-- Name: t_product_receivable_payment_pembayaran_piutang_product_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--





CREATE SEQUENCE t_product_receivable_payment_product_receivable_payment_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_product_receivable_payment_product_receivable_payment_id_seq OWNER TO postgres;

--
-- Name: t_expence; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_expence(
    expense_id t_guid NOT NULL,
    user_id t_guid,
    invoice t_invoice,
    date date,
    total t_price,
    description t_description,
    system_date timestamp without time zone DEFAULT now()
);


ALTER TABLE t_expence OWNER TO postgres;

--
-- Name: t_expence_pengeluaran_biaya_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--


CREATE SEQUENCE t_expence_expence_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_expence_expence_id_seq OWNER TO postgres;
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
    purchase_return_id t_guid NOT NULL,
    purchase_id t_guid,
    user_id t_guid,
    supplier_id t_guid,
    invoice t_invoice,
    date date,
    description t_description,
    system_date timestamp without time zone DEFAULT now(),
    total_invoice t_price
);


ALTER TABLE t_purchase_return OWNER TO postgres;

--
-- Name: t_purchase_return_retur_beli_product_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE t_purchase_return_purchase_return_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_purchase_return_purchase_return_id_seq OWNER TO postgres;

--

--
-- Name: t_sales_return; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_sales_return (
    return_sale_id t_guid NOT NULL,
    sale_id t_guid,
    user_id t_guid,
    customer_id t_guid,
    invoice t_invoice,
    date date,
    description t_description,
    system_date timestamp without time zone DEFAULT now(),
    total_invoice t_price
);


ALTER TABLE t_sales_return OWNER TO postgres;

--
-- Name: t_sales_return_retur_jual_product_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--



CREATE SEQUENCE t_sales_return_sales_return_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_sales_return_sales_return_id_seq OWNER TO postgres;


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
    ADD CONSTRAINT t_purchase_product_pkey PRIMARY KEY (purchase_id);


--
-- Name: t_employee_salary_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_employee_salary
    ADD CONSTRAINT t_employee_salary_pkey PRIMARY KEY (gaji_employee_id);


--
-- Name: t_purchase_order_item_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_purchase_order_item
    ADD CONSTRAINT t_purchase_order_item_pkey PRIMARY KEY (purchase_item_id);


--
-- Name: t_item_jual_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_sales_order_item
    ADD CONSTRAINT t_item_jual_pkey PRIMARY KEY (sale_item_id);


--
-- Name: t_debt_payment_item_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_debt_payment_item
    ADD CONSTRAINT t_debt_payment_item_pkey PRIMARY KEY (pay_purchase_item_id);


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
    ADD CONSTRAINT t_purchase_return_item_pkey PRIMARY KEY (return_purchase_item_id);


--
-- Name: t_item_retur_jual_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_sales_return_item
    ADD CONSTRAINT t_item_retur_jual_pkey PRIMARY KEY (return_sale_id);


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
    ADD CONSTRAINT t_product_payable_payment_pkey PRIMARY KEY (pay_purchase_id);


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
    ADD CONSTRAINT t_purchase_return_pkey PRIMARY KEY (purchase_return_id);


--
-- Name: t_retur_jual_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_sales_return
    ADD CONSTRAINT t_retur_jual_pkey PRIMARY KEY (return_sale_id);


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

CREATE INDEX t_beli_product_idx1 ON t_purchase_product USING btree (invoice);


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

CREATE INDEX t_gaji_employee_idx2 ON t_employee_salary USING btree (invoice);


--
-- Name: t_jual_product_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_jual_product_idx ON t_product_sales USING btree (invoice);


--
-- Name: t_jual_product_idx1; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_jual_product_idx1 ON t_product_sales USING btree (date);


--
-- Name: t_jual_product_idx2; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_jual_product_idx2 ON t_product_sales USING btree (due_date);


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

CREATE INDEX t_loan_idx1 ON t_loan USING btree (invoice);


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

CREATE INDEX t_pembayaran_hutang_product_idx1 ON t_product_payable_payment USING btree (invoice);


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

CREATE INDEX t_pembayaran_loan_idx1 ON t_pembayaran_loan USING btree (invoice);


--
-- Name: t_pembayaran_piutang_product_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_pembayaran_piutang_product_idx ON t_product_receivable_payment USING btree (date);


--
-- Name: t_pembayaran_piutang_product_idx1; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_pembayaran_piutang_product_idx1 ON t_product_receivable_payment USING btree (invoice);


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

CREATE INDEX t_expence_idx1 ON t_expence USING btree (invoice);


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

CREATE INDEX t_retur_beli_product_idx1 ON t_purchase_return USING btree (invoice);


--
-- Name: t_retur_jual_product_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_retur_jual_product_idx ON t_sales_return USING btree (date);


--
-- Name: t_retur_jual_product_idx1; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_retur_jual_product_idx1 ON t_sales_return USING btree (invoice);


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

CREATE TRIGGER tr_log_last_update AFTER UPDATE OF selling_price ON m_product FOR EACH ROW EXECUTE PROCEDURE fn_log_last_update();


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
    ADD CONSTRAINT t_purchase_product_fk2 FOREIGN KEY (purchase_return_id) REFERENCES t_purchase_return(purchase_return_id) ON UPDATE CASCADE ON DELETE SET NULL;


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
    ADD CONSTRAINT t_purchase_order_item_fk FOREIGN KEY (purchase_id) REFERENCES t_purchase_product(purchase_id) ON UPDATE CASCADE ON DELETE CASCADE;


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
    ADD CONSTRAINT t_debt_payment_item_fk FOREIGN KEY (pay_purchase_id) REFERENCES t_product_payable_payment(pay_purchase_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: t_debt_payment_item_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_debt_payment_item
    ADD CONSTRAINT t_debt_payment_item_fk1 FOREIGN KEY (purchase_id) REFERENCES t_purchase_product(purchase_id) ON UPDATE CASCADE ON DELETE CASCADE;


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
    ADD CONSTRAINT t_purchase_return_item_fk FOREIGN KEY (purchase_return_id) REFERENCES t_purchase_return(purchase_return_id) ON UPDATE CASCADE ON DELETE CASCADE;


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
    ADD CONSTRAINT t_purchase_return_item_fk3 FOREIGN KEY (purchase_item_id) REFERENCES t_purchase_order_item(purchase_item_id) ON UPDATE CASCADE;


--
-- Name: t_item_retur_jual_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_sales_return_item
    ADD CONSTRAINT t_item_retur_jual_fk FOREIGN KEY (return_sale_id) REFERENCES t_sales_return(return_sale_id) ON UPDATE CASCADE ON DELETE CASCADE;


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
    ADD CONSTRAINT t_item_retur_jual_fk3 FOREIGN KEY (sale_item_id) REFERENCES t_sales_order_item(sale_item_id) ON UPDATE CASCADE;


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
    ADD CONSTRAINT t_jual_fk2 FOREIGN KEY (return_sale_id) REFERENCES t_sales_return(return_sale_id) ON UPDATE CASCADE ON DELETE SET NULL;


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
    ADD CONSTRAINT t_purchase_return_fk FOREIGN KEY (purchase_id) REFERENCES t_purchase_product(purchase_id) ON UPDATE CASCADE ON DELETE SET NULL;


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





SET statement_timeout = 0;
SET lock_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET client_min_messages = warning;

SET search_path = public, pg_catalog;

--
-- Data for Name: m_menu; Type: TABLE DATA; Schema: public; Owner: postgres
--


COPY public.m_menu (menu_id, name_menu, menu_title, parent_id, order_number, is_active, name_form, is_enabled) FROM stdin;
0a516b56-ed73-dafe-aaa0-332fadd2f088	mnuGeneralSupplier	General Supplier	593c989d-be87-42bf-a11d-f177afcc2180	2	t	FrmGeneralSupplier	t
8a0ba72f-67d2-481f-9e10-a188f09effa5	mnuProduct	Product	07b24b4b-cf52-4b3c-ab06-51f7312e4813	3	t	FrmListProduct	t
8a8c6d23-963b-4819-819d-b9cdeaad7718	mnuCategory 	Category	07b24b4b-cf52-4b3c-ab06-51f7312e4813	2	t	FrmListCategory	t
95e9e230-c4f3-4fbc-9652-78cf4155d7ea	mnuStockAdjustment	Stock Adjustment	07b24b4b-cf52-4b3c-ab06-51f7312e4813	6	t	FrmStockAdjustment	t
fd48562f-9096-4cec-ad9c-37229fc072a3	mnuSupplier	Supplier	07b24b4b-cf52-4b3c-ab06-51f7312e4813	7	t	FrmListSupplier	t
5ab9c82d-a116-4032-8891-cbfb7b71b8e3	mnuCustomer	Customer	07b24b4b-cf52-4b3c-ab06-51f7312e4813	8	t	FrmListCustomer	t
7c7a2763-ed8b-41a7-a42d-b79233d02e02	mnuDropshipper	Dropshipper	07b24b4b-cf52-4b3c-ab06-51f7312e4813	9	t	FrmListDropshipper	t
f18fbb6e-bd6f-5d21-fa8d-11923327b436	mnuCard	Card	07b24b4b-cf52-4b3c-ab06-51f7312e4813	1	t	FrmListCard	t
fa7e83ee-9b49-4cda-badd-d68cda7b7a9a	mnuPrintingLabelBarcodeProduct	Printing Label Barcode Product	07b24b4b-cf52-4b3c-ab06-51f7312e4813	5	t	FrmPrintingLabelBarcodeProduct	t
7cca3d24-3fc3-4c64-b361-78c0c7581920	mnuPrintingLabelpriceProduct	Printing Label price Product	07b24b4b-cf52-4b3c-ab06-51f7312e4813	4	t	FrmPrintingLabelpriceProduct	t
6f13d6e5-4322-4b4b-8fca-2278b04bd4eb	mnuLapSales 	Sales	3a62ea0b-0f48-495c-947b-ad5aa9af77f7	18	t		t
a0c6daf8-b0c8-4e9d-9702-7a2bb781580c	mnuIncomeandExpense	Income and Expense	3a62ea0b-0f48-495c-947b-ad5aa9af77f7	25	t	FrmIncomeandExpense	t
65afc8bf-4df2-486a-b878-e77638ae2688	mnuLapProductStockCard	Product Stock Card	3a62ea0b-0f48-495c-947b-ad5aa9af77f7	21	t	FrmLapProductStockCard	t
d62d3352-56e7-4802-973e-32d590febdab	mnuLapProductStock	Product Stock	3a62ea0b-0f48-495c-947b-ad5aa9af77f7	19	t	FrmLapProductStock	t
986182a1-8b33-457b-9151-38676f0f0869	mnuLapStockAdjustment	Stock Adjustment	3a62ea0b-0f48-495c-947b-ad5aa9af77f7	20	t	FrmLapStockAdjustment	t
b94a2365-c063-491e-a798-c68dccd2d80b	mnuLapBestSellingProducts	Best Selling Products	3a62ea0b-0f48-495c-947b-ad5aa9af77f7	11	t	FrmLapBestSellingProducts	t
a1b976fc-99d3-4b5f-89b5-bc7e7fc4c0d2	mnuLapSalesPerCashier	Sales Per Cashier	3a62ea0b-0f48-495c-947b-ad5aa9af77f7	10	t	FrmLapSalesPerCashier	t
33d081b5-8e4d-424b-a00a-eccf1f1a8809	mnuLapCustomerProduct	Customer Product	3a62ea0b-0f48-495c-947b-ad5aa9af77f7	12	t	FrmLapCustomerProduct	t
5a114aab-28cc-4655-b676-d08075bae831	mnuLapPurchase	Purchase	3a62ea0b-0f48-495c-947b-ad5aa9af77f7	6	t		t
07b24b4b-cf52-4b3c-ab06-51f7312e4813	mnuReference	Reference	\N	1	t	\N	t
73e32548-da86-4db9-b3f9-f2ecd81ea3c9	mnuTransactions	Transactions	\N	2	t	\N	t
d1bd5f93-996c-46a3-b80f-f4f50681a1f9	mnuExpense	Expense	\N	3	t	\N	t
3a62ea0b-0f48-495c-947b-ad5aa9af77f7	mnuReport	Report	\N	4	t	\N	t
593c989d-be87-42bf-a11d-f177afcc2180	mnuSettings	Settings	\N	5	t	\N	t
a6043b21-18d0-4fcd-9ea9-f146542081d5	mnuProductPurchase	Product Purchase	73e32548-da86-4db9-b3f9-f2ecd81ea3c9	1	t	FrmListProductPurchase	t
b4be7b7c-4587-4af4-af07-fce34df723df	mnuProductSales	Product Sales	73e32548-da86-4db9-b3f9-f2ecd81ea3c9	4	t	FrmListProductSales	t
e7be0d85-9f96-4095-be35-1da049028cef	mnuTitles	Titles	07b24b4b-cf52-4b3c-ab06-51f7312e4813	10	t	FrmListTitles	t
b7ade8cc-22aa-43c8-be9c-af6cb71d11a6	mnuEmployee	Employee	07b24b4b-cf52-4b3c-ab06-51f7312e4813	11	t	FrmListEmployee	t
576b9f00-c29d-4c2c-9a6b-5a563344de93	mnuProductSalesReturn	Product Sales Return	73e32548-da86-4db9-b3f9-f2ecd81ea3c9	6	t	FrmListProductSalesReturn	t
36aabcfa-60bc-48f5-9af6-30aa7600eb5b	mnuCompanyProfile	Company Profile	593c989d-be87-42bf-a11d-f177afcc2180	1	t	FrmCompanyProfile	t
295f6ddd-b934-4215-8e44-74905efd2273	mnuLapProductPurchase	Product Purchase	3a62ea0b-0f48-495c-947b-ad5aa9af77f7	1	t	FrmLapProductPurchase	t
335e115b-8f25-4cad-96cf-22481c98c525	mnuLapProductPurchaseDebt	Product Purchase Debt	3a62ea0b-0f48-495c-947b-ad5aa9af77f7	2	t	FrmLapProductPurchaseDebt	t
42a3ffaa-da90-44d8-ae9b-cfcb7a7bdac6	mnuLapProductPurchaseDebtPayment	Product Purchase Debt Payment	3a62ea0b-0f48-495c-947b-ad5aa9af77f7	3	t	FrmLapProductPurchaseDebtPayment	t
b0b538ba-3535-4308-8012-9e2fa0daa0b0	mnuLapProductPurchaseDebtCard	Product Purchase Debt Card	3a62ea0b-0f48-495c-947b-ad5aa9af77f7	4	t	FrmLapProductPurchaseDebtCard	t
5a4b0eab-0a7b-463a-bcf8-0cca0fbddded	mnuLapProductPurchaseReturn	Product Purchase Return	3a62ea0b-0f48-495c-947b-ad5aa9af77f7	5	t	FrmLapProductPurchaseReturn	t
6a593127-4efb-4d7e-be38-53894c4828d0	mnuLapProductSales	Product Sales	3a62ea0b-0f48-495c-947b-ad5aa9af77f7	7	t	FrmLapProductSales	t
f87b0496-d9cb-4aef-b9dd-b7fe62ac3a18	mnuLapSalesperProduct	Sales per Product	3a62ea0b-0f48-495c-947b-ad5aa9af77f7	8	t	FrmLapSalesperProduct	t
5c336652-4465-4b62-8de1-dac26fc696b6	mnuLapExpense	Expense	3a62ea0b-0f48-495c-947b-ad5aa9af77f7	22	t	FrmLapExpense	t
e9e7ff66-d302-49a3-a9e5-8a02ff87e016	mnuLaploan	loan	3a62ea0b-0f48-495c-947b-ad5aa9af77f7	23	t	FrmLaploan	t
08392673-2e61-4266-a6ae-5cb75fdf42e8	mnuExpense	Expense	d1bd5f93-996c-46a3-b80f-f4f50681a1f9	1	t	FrmListExpense	t
13b929f3-d349-4686-b803-b350732003c8	mnuloan	loan	d1bd5f93-996c-46a3-b80f-f4f50681a1f9	2	t	FrmListloan	t
d5bd3b31-7feb-4c55-9fed-8d67ac18fef4	mnuEmployeeSalaryPayment	Employee Salary Payment	d1bd5f93-996c-46a3-b80f-f4f50681a1f9	3	t	FrmListEmployeeSalaryPayment	t
0702e90c-cb7c-42a4-a447-478fba5a7443	mnuApplicationAccessRights	Application Access Rights	593c989d-be87-42bf-a11d-f177afcc2180	3	t	FrmListApplicationAccessRights	t
e8f83478-a577-4cff-a06f-8d921f9367c7	mnuManajementOperator	Manajement Operator	593c989d-be87-42bf-a11d-f177afcc2180	4	t	FrmListOperator	t
942cf428-3e36-48d2-8932-7e55cde20b32	mnuLapProductSalesCredit	Product Sales Credit	3a62ea0b-0f48-495c-947b-ad5aa9af77f7	13	t	FrmLapProductSalesCredit	t
870ec1d3-5b71-47dc-b241-1b0ae933217c	mnuReturnProductPurchase	Return Product Purchase	73e32548-da86-4db9-b3f9-f2ecd81ea3c9	3	t	FrmReturnProductPurchase	t
b52e8eac-3bf6-4ebf-95a0-46ab9e7b0888	mnuTypeExpense	Expense Type	07b24b4b-cf52-4b3c-ab06-51f7312e4813	12	t	FrmListTypeExpense	t
ed926af3-61a5-40e7-8975-de78c90eb784	mnuLapSalesperCategory	Sales per Category	3a62ea0b-0f48-495c-947b-ad5aa9af77f7	9	t	FrmLapSalesPerCategory	t
99302348-4d3c-48dd-8d67-c422e3061f1c	mnuPaymentCreditSalesProduct	Payment Credit Sales Product	73e32548-da86-4db9-b3f9-f2ecd81ea3c9	5	t	FrmListPaymentCreditSalesProduct	t
8c46f173-f586-402d-b54a-ac2e4ba57f1e	mnuLapEmployeeSalaryPayment	Employee Salary Payment	3a62ea0b-0f48-495c-947b-ad5aa9af77f7	24	t	FrmLapEmployeeSalaryPayment	t
56bd7e72-5e17-4105-bf4d-5f698e94a7fb	mnuLapProductSalesReturn	Product Sales Return	3a62ea0b-0f48-495c-947b-ad5aa9af77f7	16	t	FrmLapProductSalesReturn	t
5b99b5c4-91a8-4492-80a2-71cd9b800f00	mnuLapProductSalesCreditPayment	Product Sales Credit Payment	3a62ea0b-0f48-495c-947b-ad5aa9af77f7	14	t	FrmLapProductSalesCreditPayment	t
01803718-15d1-4ef4-9574-b9bb161c1638	mnuLapProductSalesCreditCard	Card Credit Sales Product	3a62ea0b-0f48-495c-947b-ad5aa9af77f7	15	t	FrmLapProductSalesCreditCard	t
948af0c2-5c0d-4887-8d81-4cd42e1b02a0	mnuLapProfitLossSales	Sales Profit AND Loss	3a62ea0b-0f48-495c-947b-ad5aa9af77f7	17	t	FrmLapProfitLossSales	t
084488a3-092d-4e8c-8bf2-72dcf90262b4	mnuProductPurchaseDebitPayment	Product Purchase Debit Payment	73e32548-da86-4db9-b3f9-f2ecd81ea3c9	2	t	FrmListProductPurchaseDebitPayment	t
\.

--INSERT INTO m_menu (menu_id, name_menu, menu_title, parent_id, order_number, is_active, name_form, is_enabled)
--VALUES ('c9eb7f5d-0373-48b6-9b15-14e8349d9e1f', 'mnuSalesQuotation', 'Sales Quotation', '1afdbd3e-7e82-4e64-9c35-2d010a141b2f', '5', true, 'FrmListSalesQuotation', true);

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
-- Data for Name: m_item_menu; Type: TABLE DATA; Schema: public; Owner: postgres
--


COPY public.m_item_menu (item_menu_id, menu_id, grant_id, description) FROM stdin;
c4ffec93-ad1e-839a-ff41-4565e2e444bf	0a516b56-ed73-dafe-aaa0-332fadd2f088	2	Edit Data
1dabcdae-eb41-8c9b-1ab6-fc64b0f4e965	f18fbb6e-bd6f-5d21-fa8d-11923327b436	0	Melihat Data
3d92f6bc-c702-6737-6e9e-0c8e72e9ff9f	f18fbb6e-bd6f-5d21-fa8d-11923327b436	1	Tambah Data
356a458f-f27f-b912-bfb9-1a9627b14651	f18fbb6e-bd6f-5d21-fa8d-11923327b436	2	Edit Data
a994ad2b-724e-c741-0643-da10b6fe3c6b	f18fbb6e-bd6f-5d21-fa8d-11923327b436	3	Hapus Data
d0818cc8-582b-4d31-81bf-9cf1ca20fef9	a0c6daf8-b0c8-4e9d-9702-7a2bb781580c	0	Melihat Data
144d104d-d114-4bfe-b4ee-1149f0121a8f	a0c6daf8-b0c8-4e9d-9702-7a2bb781580c	1	Tambah Data
870d8e3e-c83d-4ed1-9dd6-89639cec6499	a0c6daf8-b0c8-4e9d-9702-7a2bb781580c	2	Edit Data
64c1ec17-eebb-4e31-96d1-006dad1a0721	a0c6daf8-b0c8-4e9d-9702-7a2bb781580c	3	Hapus Data
62dbc844-2271-4a5e-9ba0-f5d32308847a	b94a2365-c063-491e-a798-c68dccd2d80b	0	Melihat Data
57880071-1966-4052-976b-5ab1275787f1	b94a2365-c063-491e-a798-c68dccd2d80b	1	Tambah Data
6c4a6251-153c-49ab-b49c-64aeb47c1f70	b94a2365-c063-491e-a798-c68dccd2d80b	2	Edit Data
bf730e62-a35e-437a-803d-672af221b6bd	b94a2365-c063-491e-a798-c68dccd2d80b	3	Hapus Data
b4292bb5-48ad-4119-9138-5ffbd1443d41	a1b976fc-99d3-4b5f-89b5-bc7e7fc4c0d2	0	Melihat Data
a44b809a-91a3-44b6-aa27-4d327b0ea9ac	a1b976fc-99d3-4b5f-89b5-bc7e7fc4c0d2	1	Tambah Data
d38ee02c-15d0-4d3e-aec6-2923b650cfe4	a1b976fc-99d3-4b5f-89b5-bc7e7fc4c0d2	2	Edit Data
81e56e1f-6d9f-4114-ad88-8e55cda0c6a9	a1b976fc-99d3-4b5f-89b5-bc7e7fc4c0d2	3	Hapus Data
b2e1120f-b872-4f33-9b1b-2f93b6ced471	7c7a2763-ed8b-41a7-a42d-b79233d02e02	0	Melihat Data
1e26a1e7-9a54-4c4a-b0c8-9634420ba30f	7c7a2763-ed8b-41a7-a42d-b79233d02e02	1	Tambah Data
16949b89-1155-40ae-abf8-dbde817e4ceb	7c7a2763-ed8b-41a7-a42d-b79233d02e02	2	Edit Data
0c1fd367-ded8-49f0-adae-04fbf425cc1f	7c7a2763-ed8b-41a7-a42d-b79233d02e02	3	Hapus Data
707ab536-89c9-4967-b9e3-829a5efe62c9	33d081b5-8e4d-424b-a00a-eccf1f1a8809	0	Melihat Data
560ba445-9d7c-4bda-ae22-ef9002015b10	33d081b5-8e4d-424b-a00a-eccf1f1a8809	1	Tambah Data
8065089a-e117-49e5-a0fd-eded6141faeb	33d081b5-8e4d-424b-a00a-eccf1f1a8809	2	Edit Data
1a9f375b-d052-4c61-8c1f-4b34c21eb11e	33d081b5-8e4d-424b-a00a-eccf1f1a8809	3	Hapus Data
1e5d52e7-f0ef-46a9-b9fc-945186e39700	65afc8bf-4df2-486a-b878-e77638ae2688	0	Melihat Data
497a89a1-4f1f-4e96-8a40-f548ab72aed1	65afc8bf-4df2-486a-b878-e77638ae2688	1	Tambah Data
d56e46eb-53ae-4344-801a-81fe2459111f	65afc8bf-4df2-486a-b878-e77638ae2688	2	Edit Data
7b518174-4d11-4c68-9930-4cc32e5a1b05	65afc8bf-4df2-486a-b878-e77638ae2688	3	Hapus Data
445e6e28-6525-4ac0-ac6e-4a80f2e64fa9	7cca3d24-3fc3-4c64-b361-78c0c7581920	0	Cetak Label price Produk
623a5162-e107-41fe-bb3a-25e8d32e30ae	fa7e83ee-9b49-4cda-badd-d68cda7b7a9a	0	Cetak Barcode
e6d7d44f-a22d-4b8c-8a79-21ccf62c501c	ed926af3-61a5-40e7-8975-de78c90eb784	0	Melihat Data
51caa0e2-7004-4b5c-9eff-98d0e5cf11cb	ed926af3-61a5-40e7-8975-de78c90eb784	1	Tambah Data
d1d37165-0aad-4637-b8de-bd36b7bdecaa	ed926af3-61a5-40e7-8975-de78c90eb784	2	Edit Data
64c02357-121c-4bdc-b158-367ae6519e80	ed926af3-61a5-40e7-8975-de78c90eb784	3	Hapus Data
0fda778f-0b00-474c-95f8-b91b9759c484	948af0c2-5c0d-4887-8d81-4cd42e1b02a0	0	Melihat Data
d701f09f-1ff8-4a83-92ce-ae0080055538	948af0c2-5c0d-4887-8d81-4cd42e1b02a0	1	Tambah Data
ddf9f9c6-d23c-48d1-9896-c2c613174934	948af0c2-5c0d-4887-8d81-4cd42e1b02a0	2	Edit Data
939446e1-4185-4ef1-a072-7541a781bb58	948af0c2-5c0d-4887-8d81-4cd42e1b02a0	3	Hapus Data
fb063166-8527-4a30-b22e-69b3b97b17c8	8a8c6d23-963b-4819-819d-b9cdeaad7718	0	Melihat Data
6c5872a2-8ba5-4842-952a-3aa7f7fdae31	8a8c6d23-963b-4819-819d-b9cdeaad7718	1	Tambah Data
7931d986-3f1f-4e23-9a11-4ffb7e122067	8a8c6d23-963b-4819-819d-b9cdeaad7718	2	Edit Data
7633ad97-ce8d-468b-9b4d-af19d4bcf138	8a8c6d23-963b-4819-819d-b9cdeaad7718	3	Hapus Data
acf0a50a-277c-4c52-80a7-f0efd7026784	8a0ba72f-67d2-481f-9e10-a188f09effa5	0	Melihat Data
14900edb-6426-4f26-a9d0-aece69ccb2c1	8a0ba72f-67d2-481f-9e10-a188f09effa5	1	Tambah Data
a6429c39-0751-4cf9-8993-4d74649a1e81	8a0ba72f-67d2-481f-9e10-a188f09effa5	2	Edit Data
e15969eb-7bbc-403d-bb99-f6465393d697	8a0ba72f-67d2-481f-9e10-a188f09effa5	3	Hapus Data
0ebfb075-f973-4a5d-ad86-60411ce6a787	95e9e230-c4f3-4fbc-9652-78cf4155d7ea	0	Melihat Data
bbe4ca31-5bbd-4791-91d1-bf196a6f8f35	95e9e230-c4f3-4fbc-9652-78cf4155d7ea	1	Tambah Data
eb0bca07-ddca-4e6b-a499-49a7173330b4	95e9e230-c4f3-4fbc-9652-78cf4155d7ea	2	Edit Data
52a12c7c-34bb-46aa-9000-e5fef6afa2be	95e9e230-c4f3-4fbc-9652-78cf4155d7ea	3	Hapus Data
9aa2f5e9-f8b8-4696-9e2d-10e1118820b9	fd48562f-9096-4cec-ad9c-37229fc072a3	0	Melihat Data
b6842258-25f4-4ce1-8a62-b3ed0a5a5f1d	fd48562f-9096-4cec-ad9c-37229fc072a3	1	Tambah Data
ae432c77-b9fe-4cac-b512-3c51b70b5cdc	fd48562f-9096-4cec-ad9c-37229fc072a3	2	Edit Data
216ef3b3-b3c7-4a2c-814b-16f1e8f491dd	fd48562f-9096-4cec-ad9c-37229fc072a3	3	Hapus Data
a826173e-5ef2-4d84-80dc-1346a3b55d87	5ab9c82d-a116-4032-8891-cbfb7b71b8e3	0	Melihat Data
1dbddce5-13ee-4ef5-b30d-9fe2c6ea5702	5ab9c82d-a116-4032-8891-cbfb7b71b8e3	1	Tambah Data
9fb7dca3-649c-47cd-ae65-385a0664b3e4	5ab9c82d-a116-4032-8891-cbfb7b71b8e3	2	Edit Data
cc3ffcb4-0ce5-4b5a-8a48-7997629df57d	5ab9c82d-a116-4032-8891-cbfb7b71b8e3	3	Hapus Data
1e5d489a-6091-4285-88cd-33b543faeee0	e7be0d85-9f96-4095-be35-1da049028cef	0	Melihat Data
cc1039a5-fd4b-47fc-bf8a-8e6ffaae0d64	e7be0d85-9f96-4095-be35-1da049028cef	1	Tambah Data
d3473ff5-faa9-42ca-992c-78c7171e48af	e7be0d85-9f96-4095-be35-1da049028cef	2	Edit Data
8026dd31-bf35-4a86-a5bf-8b5d9554ab9e	e7be0d85-9f96-4095-be35-1da049028cef	3	Hapus Data
95e679e8-0716-4a8a-a560-ab578e25c20f	b7ade8cc-22aa-43c8-be9c-af6cb71d11a6	0	Melihat Data
1adfa5d6-6769-466b-aaa9-2b923a939945	b7ade8cc-22aa-43c8-be9c-af6cb71d11a6	1	Tambah Data
46cebf0e-27e1-444d-a2a3-84eb0e21a3d3	b7ade8cc-22aa-43c8-be9c-af6cb71d11a6	2	Edit Data
86503188-5b09-4132-a1f3-b38cb63c6786	b7ade8cc-22aa-43c8-be9c-af6cb71d11a6	3	Hapus Data
818f879b-157c-472c-934b-745cbd7389fe	b52e8eac-3bf6-4ebf-95a0-46ab9e7b0888	0	Melihat Data
45f2437b-e741-4bf4-97e2-220db7af6171	b52e8eac-3bf6-4ebf-95a0-46ab9e7b0888	1	Tambah Data
f7c9d53c-764f-4feb-9188-e1c5401a0a6a	b52e8eac-3bf6-4ebf-95a0-46ab9e7b0888	2	Edit Data
c0c1cc14-8b9c-4dd9-9435-b4cc777b834f	b52e8eac-3bf6-4ebf-95a0-46ab9e7b0888	3	Hapus Data
ef2d0758-cbf1-4c2a-8887-eb022d316b2f	a6043b21-18d0-4fcd-9ea9-f146542081d5	0	Melihat Data
1977fd24-fb8c-443e-be71-1067e7266a57	a6043b21-18d0-4fcd-9ea9-f146542081d5	1	Tambah Data
686eed4d-08cc-4e00-8edf-9ae0041613df	a6043b21-18d0-4fcd-9ea9-f146542081d5	2	Edit Data
9cfad67a-73c3-4b8c-852c-aa3c3baf5d7f	a6043b21-18d0-4fcd-9ea9-f146542081d5	3	Hapus Data
9988ecb3-6768-4b01-b1db-449c81c6b9d4	084488a3-092d-4e8c-8bf2-72dcf90262b4	0	Melihat Data
2b937e8d-2395-4f39-866d-b1569bc91faf	084488a3-092d-4e8c-8bf2-72dcf90262b4	1	Tambah Data
94cff94e-29f1-4905-a840-df6422552162	084488a3-092d-4e8c-8bf2-72dcf90262b4	2	Edit Data
60093fb2-340b-45f5-a1ff-5ca6b8d9a13e	084488a3-092d-4e8c-8bf2-72dcf90262b4	3	Hapus Data
579bb10a-b19a-4cfd-ae62-799ef9c5fe71	870ec1d3-5b71-47dc-b241-1b0ae933217c	0	Melihat Data
40b82838-fa26-431e-89f1-1ca918a11c40	870ec1d3-5b71-47dc-b241-1b0ae933217c	1	Tambah Data
e9e52f8b-9dcb-4953-9c69-31981385d49d	870ec1d3-5b71-47dc-b241-1b0ae933217c	2	Edit Data
9193e4c5-958c-478c-96b8-0902ea6cd917	870ec1d3-5b71-47dc-b241-1b0ae933217c	3	Hapus Data
3f8caa2e-136e-4a3c-bc98-2c00209a364b	b4be7b7c-4587-4af4-af07-fce34df723df	0	Melihat Data
27ba706a-cfa5-4814-acec-b759816d0a40	b4be7b7c-4587-4af4-af07-fce34df723df	1	Tambah Data
9bbf8a5e-1366-4bb1-81e4-b403914a107d	b4be7b7c-4587-4af4-af07-fce34df723df	2	Edit Data
d7a4b2f8-e1ad-4bef-a615-26a922f03141	b4be7b7c-4587-4af4-af07-fce34df723df	3	Hapus Data
c49e137e-cdca-4507-8d09-24fbe083449b	99302348-4d3c-48dd-8d67-c422e3061f1c	0	Melihat Data
4fccd5ce-39cd-4dac-bd0d-7c03ba2e5b78	99302348-4d3c-48dd-8d67-c422e3061f1c	1	Tambah Data
a65ae8d6-1645-4df8-8dfb-d31d253035e5	99302348-4d3c-48dd-8d67-c422e3061f1c	2	Edit Data
cbaf9fa6-e958-482d-a645-960dd2f78f66	99302348-4d3c-48dd-8d67-c422e3061f1c	3	Hapus Data
47424a6a-d185-443f-a227-b81014af1aff	576b9f00-c29d-4c2c-9a6b-5a563344de93	0	Melihat Data
c1bb7b36-efe8-4740-8e18-4f56d064481a	576b9f00-c29d-4c2c-9a6b-5a563344de93	1	Tambah Data
6d2b565d-eb55-4da1-a987-04eca17099ab	576b9f00-c29d-4c2c-9a6b-5a563344de93	2	Edit Data
5cade8ec-00eb-4d8f-ae94-3a7d17a64451	576b9f00-c29d-4c2c-9a6b-5a563344de93	3	Hapus Data
b931c11c-6051-4aac-8f11-f0dd368985d7	08392673-2e61-4266-a6ae-5cb75fdf42e8	0	Melihat Data
e259ac80-3176-4a82-a640-d1c2c53752f3	08392673-2e61-4266-a6ae-5cb75fdf42e8	1	Tambah Data
226b6941-ee52-4a8e-9b02-215a7e1d5c37	08392673-2e61-4266-a6ae-5cb75fdf42e8	2	Edit Data
464336fe-e2ac-47af-a0bc-960efbe8f2be	08392673-2e61-4266-a6ae-5cb75fdf42e8	3	Hapus Data
8b939067-c6ac-4c2f-abed-11add45bb6d3	13b929f3-d349-4686-b803-b350732003c8	0	Melihat Data
5c81d781-8429-4d26-970e-f32a2b4845ad	13b929f3-d349-4686-b803-b350732003c8	1	Tambah Data
157445a9-9189-45a2-9fba-26c81fc2a498	13b929f3-d349-4686-b803-b350732003c8	2	Edit Data
43dbb63b-6259-4b1e-94ee-fbefa6dbd4aa	13b929f3-d349-4686-b803-b350732003c8	3	Hapus Data
a990873c-bc20-4365-a747-ccc0a66fb6b2	d5bd3b31-7feb-4c55-9fed-8d67ac18fef4	0	Melihat Data
f0e18302-98f3-4658-91cb-03dc1ad17555	d5bd3b31-7feb-4c55-9fed-8d67ac18fef4	1	Tambah Data
1825820a-040c-41dd-bfd3-1193dc0b03a6	d5bd3b31-7feb-4c55-9fed-8d67ac18fef4	2	Edit Data
ef115fab-edaa-414f-8c9e-0f0f81cc2f4d	d5bd3b31-7feb-4c55-9fed-8d67ac18fef4	3	Hapus Data
853d4604-5e44-4f64-aa53-a10af1fd8126	295f6ddd-b934-4215-8e44-74905efd2273	0	Melihat Data
9c83adeb-b152-49b0-87da-e7fad85464ab	295f6ddd-b934-4215-8e44-74905efd2273	1	Tambah Data
96fbe49d-2ffb-4541-91d8-f63c02ea016d	295f6ddd-b934-4215-8e44-74905efd2273	2	Edit Data
a18cadbd-3127-49a9-935e-e36a123d0fe4	295f6ddd-b934-4215-8e44-74905efd2273	3	Hapus Data
7b730f8f-8be7-46e8-8ecd-fc6cd9c353f3	335e115b-8f25-4cad-96cf-22481c98c525	0	Melihat Data
07aaad91-b21a-4033-b54f-8030ae8fd9bd	335e115b-8f25-4cad-96cf-22481c98c525	1	Tambah Data
838dafdf-527c-428b-a071-6578be6fdc38	335e115b-8f25-4cad-96cf-22481c98c525	2	Edit Data
a64354e6-f81d-4753-a1e4-2840b05464b4	335e115b-8f25-4cad-96cf-22481c98c525	3	Hapus Data
91e62019-5f2f-408f-bfa8-6577d8d9e495	42a3ffaa-da90-44d8-ae9b-cfcb7a7bdac6	0	Melihat Data
d115d67d-2760-41d2-9d38-46c108759217	42a3ffaa-da90-44d8-ae9b-cfcb7a7bdac6	1	Tambah Data
2db01574-2ffc-4cd6-b491-0b139cf86957	42a3ffaa-da90-44d8-ae9b-cfcb7a7bdac6	2	Edit Data
3693f7b8-2dc1-4b37-b94a-6ecaf47b700d	42a3ffaa-da90-44d8-ae9b-cfcb7a7bdac6	3	Hapus Data
217210c0-1190-4e02-9407-4877304bd86e	b0b538ba-3535-4308-8012-9e2fa0daa0b0	0	Melihat Data
6b01ff73-47e4-4a07-a670-4d45841251be	b0b538ba-3535-4308-8012-9e2fa0daa0b0	1	Tambah Data
cd3fb679-a3da-42cb-89d7-f684382ebd02	b0b538ba-3535-4308-8012-9e2fa0daa0b0	2	Edit Data
70ea7e61-b3c2-4c74-85f9-f3941e59e030	b0b538ba-3535-4308-8012-9e2fa0daa0b0	3	Hapus Data
e97ae76a-3c59-4f95-b896-ec757020e5d3	5a4b0eab-0a7b-463a-bcf8-0cca0fbddded	0	Melihat Data
0d3cafe1-5d85-48d9-8ff6-7759f059f72d	5a4b0eab-0a7b-463a-bcf8-0cca0fbddded	1	Tambah Data
7550e753-02c9-45ed-b105-3b9714fc0340	5a4b0eab-0a7b-463a-bcf8-0cca0fbddded	2	Edit Data
14b47dd7-05c3-4bf3-b2a9-6fcc634d120e	5a4b0eab-0a7b-463a-bcf8-0cca0fbddded	3	Hapus Data
68b7c124-dfb7-4631-828f-13f0c4173aef	5a114aab-28cc-4655-b676-d08075bae831	0	Melihat Data
40e58353-6bfe-4203-b2d0-d03f01dba41a	5a114aab-28cc-4655-b676-d08075bae831	1	Tambah Data
653d9faf-4a7c-4c47-99ca-2d00bb4f60a4	5a114aab-28cc-4655-b676-d08075bae831	2	Edit Data
439241e4-8aad-4563-ae01-7cfbbcad2674	5a114aab-28cc-4655-b676-d08075bae831	3	Hapus Data
933f45ce-bf98-4a20-b512-9f716c3ec348	6a593127-4efb-4d7e-be38-53894c4828d0	0	Melihat Data
0a482b87-7636-4ad4-b6a4-05b592f91022	6a593127-4efb-4d7e-be38-53894c4828d0	1	Tambah Data
b1ca012a-15e2-41b3-95c0-ca3262e34015	6a593127-4efb-4d7e-be38-53894c4828d0	2	Edit Data
00c3da69-773a-4adc-acbd-215bbba8ede4	6a593127-4efb-4d7e-be38-53894c4828d0	3	Hapus Data
dd2d3e8a-4113-44a2-ade3-39914dbd392a	f87b0496-d9cb-4aef-b9dd-b7fe62ac3a18	0	Melihat Data
73d4fadb-ea43-42eb-90a3-2828a21669eb	f87b0496-d9cb-4aef-b9dd-b7fe62ac3a18	1	Tambah Data
0be0e4bc-2ac4-4266-88e5-509bc1b716ca	f87b0496-d9cb-4aef-b9dd-b7fe62ac3a18	2	Edit Data
9445fd75-ed03-4848-a901-4fddc110f87c	f87b0496-d9cb-4aef-b9dd-b7fe62ac3a18	3	Hapus Data
b8bfb211-314f-462f-bf62-530d06a5037f	942cf428-3e36-48d2-8932-7e55cde20b32	0	Melihat Data
02e6c335-98e4-4f50-9616-ea9dc6c7367e	942cf428-3e36-48d2-8932-7e55cde20b32	1	Tambah Data
fe829c88-ebf5-46db-a3f6-484244debcb3	942cf428-3e36-48d2-8932-7e55cde20b32	2	Edit Data
7f2e8695-42c6-47d1-ab99-03a840e9e696	942cf428-3e36-48d2-8932-7e55cde20b32	3	Hapus Data
bbb3be48-2522-435b-9673-b76ae299acd3	5b99b5c4-91a8-4492-80a2-71cd9b800f00	0	Melihat Data
13835216-f64e-443f-8379-404dfc1d9536	5b99b5c4-91a8-4492-80a2-71cd9b800f00	1	Tambah Data
2fe7ed54-fe09-47da-a7cd-9a11e3414917	5b99b5c4-91a8-4492-80a2-71cd9b800f00	2	Edit Data
b975a20b-95ba-4760-b3b0-c2111abd7ae0	5b99b5c4-91a8-4492-80a2-71cd9b800f00	3	Hapus Data
f748bb4c-157c-4b34-bd15-915180eb08d6	01803718-15d1-4ef4-9574-b9bb161c1638	0	Melihat Data
5d46b875-0d13-434d-bf69-241b8a498aad	01803718-15d1-4ef4-9574-b9bb161c1638	1	Tambah Data
5e8bc06d-a358-4293-8db6-9715d94fa8fc	01803718-15d1-4ef4-9574-b9bb161c1638	2	Edit Data
d1c3a0b4-039d-4726-bdde-b2617f9d10dd	01803718-15d1-4ef4-9574-b9bb161c1638	3	Hapus Data
2f955886-03c6-40bd-af97-e3fc7e8e7450	56bd7e72-5e17-4105-bf4d-5f698e94a7fb	0	Melihat Data
87e413b7-d136-421d-abc8-3b0d2dd01271	56bd7e72-5e17-4105-bf4d-5f698e94a7fb	1	Tambah Data
76c4bd40-4e12-4172-bdca-c9510eeebf57	56bd7e72-5e17-4105-bf4d-5f698e94a7fb	2	Edit Data
5b90db87-7e41-4485-88fd-23198ad2b139	56bd7e72-5e17-4105-bf4d-5f698e94a7fb	3	Hapus Data
ae3dc6c4-e67e-43d7-82bc-3ef5d968b5a4	6f13d6e5-4322-4b4b-8fca-2278b04bd4eb	0	Melihat Data
0f350585-53ca-4f1c-95e1-a02b4946a350	6f13d6e5-4322-4b4b-8fca-2278b04bd4eb	1	Tambah Data
5a1def48-492c-4438-960d-8e3662902701	6f13d6e5-4322-4b4b-8fca-2278b04bd4eb	2	Edit Data
11adf051-48f6-4f61-a89d-c547210dcc23	6f13d6e5-4322-4b4b-8fca-2278b04bd4eb	3	Hapus Data
e7f230bf-f7df-44c3-82a1-41c0a21b1813	d62d3352-56e7-4802-973e-32d590febdab	0	Melihat Data
f54944cf-d7c9-4e3c-8d49-1cb264a12852	d62d3352-56e7-4802-973e-32d590febdab	1	Tambah Data
25843563-bc00-4235-813a-b09f74185066	d62d3352-56e7-4802-973e-32d590febdab	2	Edit Data
1ea4681f-5a75-474d-9f4a-6cf11c917745	d62d3352-56e7-4802-973e-32d590febdab	3	Hapus Data
4f1fda44-3780-4c6b-a568-f1bbd83d60eb	986182a1-8b33-457b-9151-38676f0f0869	0	Melihat Data
7b81dff2-64de-43ee-b3d7-172b6ad8bbf4	986182a1-8b33-457b-9151-38676f0f0869	1	Tambah Data
b967d07d-c0c5-4686-a3e2-cd29288c35a9	986182a1-8b33-457b-9151-38676f0f0869	2	Edit Data
74befc13-6931-464f-b021-5a996b999f24	986182a1-8b33-457b-9151-38676f0f0869	3	Hapus Data
e6f0a448-6962-4815-b4c6-c24766b16bf9	5c336652-4465-4b62-8de1-dac26fc696b6	0	Melihat Data
bd6c0115-736d-48a2-b5f3-2552bbfc7580	5c336652-4465-4b62-8de1-dac26fc696b6	1	Tambah Data
39d6330a-23f4-431b-a497-7dab1de4464a	5c336652-4465-4b62-8de1-dac26fc696b6	2	Edit Data
f1407b4b-5f5e-480d-b94a-7440d77688bc	5c336652-4465-4b62-8de1-dac26fc696b6	3	Hapus Data
67bfefed-1ea1-4e9a-ae61-3dd8b0db9e98	e9e7ff66-d302-49a3-a9e5-8a02ff87e016	0	Melihat Data
b36298e7-9bfd-417d-9ce7-aa18966c0394	e9e7ff66-d302-49a3-a9e5-8a02ff87e016	1	Tambah Data
ac60965e-8a2d-4bc6-aab1-bf85eb40409a	e9e7ff66-d302-49a3-a9e5-8a02ff87e016	2	Edit Data
123a1c60-1c56-473f-bc7c-c1f8e76f5615	e9e7ff66-d302-49a3-a9e5-8a02ff87e016	3	Hapus Data
83ecbd41-11ee-497f-b34a-2dda5bcc2c72	8c46f173-f586-402d-b54a-ac2e4ba57f1e	0	Melihat Data
83777d90-22a0-4a63-92f9-5e594d92d03c	8c46f173-f586-402d-b54a-ac2e4ba57f1e	1	Tambah Data
845ca650-9956-4b63-88c4-417628f73c18	8c46f173-f586-402d-b54a-ac2e4ba57f1e	2	Edit Data
a250ef10-430d-4866-a032-98ac65b835e8	8c46f173-f586-402d-b54a-ac2e4ba57f1e	3	Hapus Data
06c6e20f-934a-46c9-a74b-76864cdc7c69	36aabcfa-60bc-48f5-9af6-30aa7600eb5b	2	Edit Data
61e5a064-bd9b-4018-a1be-f0fc65ebe422	0702e90c-cb7c-42a4-a447-478fba5a7443	0	Melihat Data
aff995dc-fd33-4d0d-a5cb-d7b23a9298a9	0702e90c-cb7c-42a4-a447-478fba5a7443	1	Tambah Data
4904e829-b642-4485-96a3-5c415cb5dc7b	0702e90c-cb7c-42a4-a447-478fba5a7443	2	Edit Data
2becf456-45e4-4645-8a40-2cc8525025d7	0702e90c-cb7c-42a4-a447-478fba5a7443	3	Hapus Data
db7e5b95-2f20-4317-aba2-2030a4de822a	e8f83478-a577-4cff-a06f-8d921f9367c7	0	Melihat Data
f62ee5a7-db04-4341-ae60-01d73868226a	e8f83478-a577-4cff-a06f-8d921f9367c7	1	Tambah Data
ff49859c-5c15-4d80-8c62-13e1b1668365	e8f83478-a577-4cff-a06f-8d921f9367c7	2	Edit Data
3b6d8954-39c7-44f8-9ff5-49e477876562	e8f83478-a577-4cff-a06f-8d921f9367c7	3	Hapus Data
\.



--
-- PostgreSQL database dump
--

-- Dumped from database version 15.2
-- Dumped by pg_dump version 15.2

SET statement_timeout = 0;
SET lock_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET client_min_messages = warning;

SET search_path = public, pg_catalog;

--
-- Name: m_role; Type: TABLE; Schema: public; Owner: postgres
--

--
-- Data for Name: m_role; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.m_role (role_id, name_role, is_active) FROM stdin;
11dc1faf-2c66-4525-932d-a90e24da8987	Administrator	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	Owner	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	Kasir	t
29656af0-c3f6-44e8-9c74-c3643f38e871	Supervisor	t
\.




--
-- PostgreSQL database dump complete
--



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
-- Data for Name: m_role_privilege; Type: TABLE DATA; Schema: public; Owner: postgres

COPY public.m_role_privilege (role_id, menu_id, grant_id, is_grant) FROM stdin;
11dc1faf-2c66-4525-932d-a90e24da8987	8a8c6d23-963b-4819-819d-b9cdeaad7718	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	8a8c6d23-963b-4819-819d-b9cdeaad7718	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	8a8c6d23-963b-4819-819d-b9cdeaad7718	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	8a8c6d23-963b-4819-819d-b9cdeaad7718	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	8a0ba72f-67d2-481f-9e10-a188f09effa5	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	8a0ba72f-67d2-481f-9e10-a188f09effa5	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	8a0ba72f-67d2-481f-9e10-a188f09effa5	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	8a0ba72f-67d2-481f-9e10-a188f09effa5	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	95e9e230-c4f3-4fbc-9652-78cf4155d7ea	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	95e9e230-c4f3-4fbc-9652-78cf4155d7ea	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	95e9e230-c4f3-4fbc-9652-78cf4155d7ea	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	95e9e230-c4f3-4fbc-9652-78cf4155d7ea	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	fd48562f-9096-4cec-ad9c-37229fc072a3	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	fd48562f-9096-4cec-ad9c-37229fc072a3	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	fd48562f-9096-4cec-ad9c-37229fc072a3	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	fd48562f-9096-4cec-ad9c-37229fc072a3	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	5ab9c82d-a116-4032-8891-cbfb7b71b8e3	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	5ab9c82d-a116-4032-8891-cbfb7b71b8e3	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	5ab9c82d-a116-4032-8891-cbfb7b71b8e3	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	5ab9c82d-a116-4032-8891-cbfb7b71b8e3	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	e7be0d85-9f96-4095-be35-1da049028cef	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	e7be0d85-9f96-4095-be35-1da049028cef	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	e7be0d85-9f96-4095-be35-1da049028cef	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	e7be0d85-9f96-4095-be35-1da049028cef	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	b7ade8cc-22aa-43c8-be9c-af6cb71d11a6	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	b7ade8cc-22aa-43c8-be9c-af6cb71d11a6	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	b7ade8cc-22aa-43c8-be9c-af6cb71d11a6	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	b7ade8cc-22aa-43c8-be9c-af6cb71d11a6	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	b52e8eac-3bf6-4ebf-95a0-46ab9e7b0888	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	b52e8eac-3bf6-4ebf-95a0-46ab9e7b0888	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	b52e8eac-3bf6-4ebf-95a0-46ab9e7b0888	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	b52e8eac-3bf6-4ebf-95a0-46ab9e7b0888	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	a6043b21-18d0-4fcd-9ea9-f146542081d5	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	a6043b21-18d0-4fcd-9ea9-f146542081d5	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	a6043b21-18d0-4fcd-9ea9-f146542081d5	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	a6043b21-18d0-4fcd-9ea9-f146542081d5	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	084488a3-092d-4e8c-8bf2-72dcf90262b4	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	084488a3-092d-4e8c-8bf2-72dcf90262b4	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	084488a3-092d-4e8c-8bf2-72dcf90262b4	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	084488a3-092d-4e8c-8bf2-72dcf90262b4	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	870ec1d3-5b71-47dc-b241-1b0ae933217c	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	870ec1d3-5b71-47dc-b241-1b0ae933217c	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	870ec1d3-5b71-47dc-b241-1b0ae933217c	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	870ec1d3-5b71-47dc-b241-1b0ae933217c	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	b4be7b7c-4587-4af4-af07-fce34df723df	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	b4be7b7c-4587-4af4-af07-fce34df723df	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	b4be7b7c-4587-4af4-af07-fce34df723df	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	b4be7b7c-4587-4af4-af07-fce34df723df	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	99302348-4d3c-48dd-8d67-c422e3061f1c	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	99302348-4d3c-48dd-8d67-c422e3061f1c	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	99302348-4d3c-48dd-8d67-c422e3061f1c	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	99302348-4d3c-48dd-8d67-c422e3061f1c	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	576b9f00-c29d-4c2c-9a6b-5a563344de93	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	576b9f00-c29d-4c2c-9a6b-5a563344de93	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	576b9f00-c29d-4c2c-9a6b-5a563344de93	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	576b9f00-c29d-4c2c-9a6b-5a563344de93	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	08392673-2e61-4266-a6ae-5cb75fdf42e8	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	08392673-2e61-4266-a6ae-5cb75fdf42e8	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	08392673-2e61-4266-a6ae-5cb75fdf42e8	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	08392673-2e61-4266-a6ae-5cb75fdf42e8	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	13b929f3-d349-4686-b803-b350732003c8	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	13b929f3-d349-4686-b803-b350732003c8	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	13b929f3-d349-4686-b803-b350732003c8	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	13b929f3-d349-4686-b803-b350732003c8	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	d5bd3b31-7feb-4c55-9fed-8d67ac18fef4	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	d5bd3b31-7feb-4c55-9fed-8d67ac18fef4	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	d5bd3b31-7feb-4c55-9fed-8d67ac18fef4	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	d5bd3b31-7feb-4c55-9fed-8d67ac18fef4	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	295f6ddd-b934-4215-8e44-74905efd2273	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	295f6ddd-b934-4215-8e44-74905efd2273	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	295f6ddd-b934-4215-8e44-74905efd2273	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	295f6ddd-b934-4215-8e44-74905efd2273	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	335e115b-8f25-4cad-96cf-22481c98c525	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	335e115b-8f25-4cad-96cf-22481c98c525	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	335e115b-8f25-4cad-96cf-22481c98c525	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	335e115b-8f25-4cad-96cf-22481c98c525	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	42a3ffaa-da90-44d8-ae9b-cfcb7a7bdac6	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	42a3ffaa-da90-44d8-ae9b-cfcb7a7bdac6	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	42a3ffaa-da90-44d8-ae9b-cfcb7a7bdac6	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	42a3ffaa-da90-44d8-ae9b-cfcb7a7bdac6	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	b0b538ba-3535-4308-8012-9e2fa0daa0b0	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	b0b538ba-3535-4308-8012-9e2fa0daa0b0	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	b0b538ba-3535-4308-8012-9e2fa0daa0b0	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	b0b538ba-3535-4308-8012-9e2fa0daa0b0	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	5a4b0eab-0a7b-463a-bcf8-0cca0fbddded	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	5a4b0eab-0a7b-463a-bcf8-0cca0fbddded	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	5a4b0eab-0a7b-463a-bcf8-0cca0fbddded	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	5a4b0eab-0a7b-463a-bcf8-0cca0fbddded	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	5a114aab-28cc-4655-b676-d08075bae831	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	5a114aab-28cc-4655-b676-d08075bae831	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	5a114aab-28cc-4655-b676-d08075bae831	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	5a114aab-28cc-4655-b676-d08075bae831	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	6a593127-4efb-4d7e-be38-53894c4828d0	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	6a593127-4efb-4d7e-be38-53894c4828d0	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	6a593127-4efb-4d7e-be38-53894c4828d0	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	6a593127-4efb-4d7e-be38-53894c4828d0	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	f87b0496-d9cb-4aef-b9dd-b7fe62ac3a18	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	f87b0496-d9cb-4aef-b9dd-b7fe62ac3a18	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	f87b0496-d9cb-4aef-b9dd-b7fe62ac3a18	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	f87b0496-d9cb-4aef-b9dd-b7fe62ac3a18	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	942cf428-3e36-48d2-8932-7e55cde20b32	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	942cf428-3e36-48d2-8932-7e55cde20b32	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	942cf428-3e36-48d2-8932-7e55cde20b32	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	942cf428-3e36-48d2-8932-7e55cde20b32	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	5b99b5c4-91a8-4492-80a2-71cd9b800f00	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	5b99b5c4-91a8-4492-80a2-71cd9b800f00	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	5b99b5c4-91a8-4492-80a2-71cd9b800f00	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	5b99b5c4-91a8-4492-80a2-71cd9b800f00	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	01803718-15d1-4ef4-9574-b9bb161c1638	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	01803718-15d1-4ef4-9574-b9bb161c1638	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	01803718-15d1-4ef4-9574-b9bb161c1638	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	01803718-15d1-4ef4-9574-b9bb161c1638	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	56bd7e72-5e17-4105-bf4d-5f698e94a7fb	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	56bd7e72-5e17-4105-bf4d-5f698e94a7fb	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	56bd7e72-5e17-4105-bf4d-5f698e94a7fb	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	56bd7e72-5e17-4105-bf4d-5f698e94a7fb	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	6f13d6e5-4322-4b4b-8fca-2278b04bd4eb	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	6f13d6e5-4322-4b4b-8fca-2278b04bd4eb	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	6f13d6e5-4322-4b4b-8fca-2278b04bd4eb	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	6f13d6e5-4322-4b4b-8fca-2278b04bd4eb	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	d62d3352-56e7-4802-973e-32d590febdab	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	d62d3352-56e7-4802-973e-32d590febdab	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	d62d3352-56e7-4802-973e-32d590febdab	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	d62d3352-56e7-4802-973e-32d590febdab	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	986182a1-8b33-457b-9151-38676f0f0869	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	986182a1-8b33-457b-9151-38676f0f0869	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	986182a1-8b33-457b-9151-38676f0f0869	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	986182a1-8b33-457b-9151-38676f0f0869	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	5c336652-4465-4b62-8de1-dac26fc696b6	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	5c336652-4465-4b62-8de1-dac26fc696b6	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	5c336652-4465-4b62-8de1-dac26fc696b6	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	5c336652-4465-4b62-8de1-dac26fc696b6	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	e9e7ff66-d302-49a3-a9e5-8a02ff87e016	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	e9e7ff66-d302-49a3-a9e5-8a02ff87e016	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	e9e7ff66-d302-49a3-a9e5-8a02ff87e016	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	e9e7ff66-d302-49a3-a9e5-8a02ff87e016	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	8c46f173-f586-402d-b54a-ac2e4ba57f1e	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	8c46f173-f586-402d-b54a-ac2e4ba57f1e	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	8c46f173-f586-402d-b54a-ac2e4ba57f1e	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	8c46f173-f586-402d-b54a-ac2e4ba57f1e	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	36aabcfa-60bc-48f5-9af6-30aa7600eb5b	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	0702e90c-cb7c-42a4-a447-478fba5a7443	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	0702e90c-cb7c-42a4-a447-478fba5a7443	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	0702e90c-cb7c-42a4-a447-478fba5a7443	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	0702e90c-cb7c-42a4-a447-478fba5a7443	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	e8f83478-a577-4cff-a06f-8d921f9367c7	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	e8f83478-a577-4cff-a06f-8d921f9367c7	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	e8f83478-a577-4cff-a06f-8d921f9367c7	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	e8f83478-a577-4cff-a06f-8d921f9367c7	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	7cca3d24-3fc3-4c64-b361-78c0c7581920	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	a6043b21-18d0-4fcd-9ea9-f146542081d5	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	a6043b21-18d0-4fcd-9ea9-f146542081d5	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	a6043b21-18d0-4fcd-9ea9-f146542081d5	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	a6043b21-18d0-4fcd-9ea9-f146542081d5	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	084488a3-092d-4e8c-8bf2-72dcf90262b4	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	084488a3-092d-4e8c-8bf2-72dcf90262b4	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	084488a3-092d-4e8c-8bf2-72dcf90262b4	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	084488a3-092d-4e8c-8bf2-72dcf90262b4	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	870ec1d3-5b71-47dc-b241-1b0ae933217c	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	870ec1d3-5b71-47dc-b241-1b0ae933217c	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	870ec1d3-5b71-47dc-b241-1b0ae933217c	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	870ec1d3-5b71-47dc-b241-1b0ae933217c	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	b4be7b7c-4587-4af4-af07-fce34df723df	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	b4be7b7c-4587-4af4-af07-fce34df723df	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	b4be7b7c-4587-4af4-af07-fce34df723df	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	b4be7b7c-4587-4af4-af07-fce34df723df	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	99302348-4d3c-48dd-8d67-c422e3061f1c	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	99302348-4d3c-48dd-8d67-c422e3061f1c	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	99302348-4d3c-48dd-8d67-c422e3061f1c	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	99302348-4d3c-48dd-8d67-c422e3061f1c	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	576b9f00-c29d-4c2c-9a6b-5a563344de93	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	576b9f00-c29d-4c2c-9a6b-5a563344de93	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	576b9f00-c29d-4c2c-9a6b-5a563344de93	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	576b9f00-c29d-4c2c-9a6b-5a563344de93	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	08392673-2e61-4266-a6ae-5cb75fdf42e8	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	08392673-2e61-4266-a6ae-5cb75fdf42e8	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	08392673-2e61-4266-a6ae-5cb75fdf42e8	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	08392673-2e61-4266-a6ae-5cb75fdf42e8	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	13b929f3-d349-4686-b803-b350732003c8	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	8a8c6d23-963b-4819-819d-b9cdeaad7718	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	8a8c6d23-963b-4819-819d-b9cdeaad7718	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	8a8c6d23-963b-4819-819d-b9cdeaad7718	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	8a0ba72f-67d2-481f-9e10-a188f09effa5	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	8a0ba72f-67d2-481f-9e10-a188f09effa5	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	8a0ba72f-67d2-481f-9e10-a188f09effa5	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	8a0ba72f-67d2-481f-9e10-a188f09effa5	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	95e9e230-c4f3-4fbc-9652-78cf4155d7ea	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	95e9e230-c4f3-4fbc-9652-78cf4155d7ea	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	95e9e230-c4f3-4fbc-9652-78cf4155d7ea	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	95e9e230-c4f3-4fbc-9652-78cf4155d7ea	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	fd48562f-9096-4cec-ad9c-37229fc072a3	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	fd48562f-9096-4cec-ad9c-37229fc072a3	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	fd48562f-9096-4cec-ad9c-37229fc072a3	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	5ab9c82d-a116-4032-8891-cbfb7b71b8e3	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	5ab9c82d-a116-4032-8891-cbfb7b71b8e3	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	5ab9c82d-a116-4032-8891-cbfb7b71b8e3	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	5ab9c82d-a116-4032-8891-cbfb7b71b8e3	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	e7be0d85-9f96-4095-be35-1da049028cef	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	e7be0d85-9f96-4095-be35-1da049028cef	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	e7be0d85-9f96-4095-be35-1da049028cef	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	e7be0d85-9f96-4095-be35-1da049028cef	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	b7ade8cc-22aa-43c8-be9c-af6cb71d11a6	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	b7ade8cc-22aa-43c8-be9c-af6cb71d11a6	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	b7ade8cc-22aa-43c8-be9c-af6cb71d11a6	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	b7ade8cc-22aa-43c8-be9c-af6cb71d11a6	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	b52e8eac-3bf6-4ebf-95a0-46ab9e7b0888	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	b52e8eac-3bf6-4ebf-95a0-46ab9e7b0888	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	b52e8eac-3bf6-4ebf-95a0-46ab9e7b0888	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	b52e8eac-3bf6-4ebf-95a0-46ab9e7b0888	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	13b929f3-d349-4686-b803-b350732003c8	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	13b929f3-d349-4686-b803-b350732003c8	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	13b929f3-d349-4686-b803-b350732003c8	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	d5bd3b31-7feb-4c55-9fed-8d67ac18fef4	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	d5bd3b31-7feb-4c55-9fed-8d67ac18fef4	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	d5bd3b31-7feb-4c55-9fed-8d67ac18fef4	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	d5bd3b31-7feb-4c55-9fed-8d67ac18fef4	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	295f6ddd-b934-4215-8e44-74905efd2273	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	295f6ddd-b934-4215-8e44-74905efd2273	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	295f6ddd-b934-4215-8e44-74905efd2273	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	295f6ddd-b934-4215-8e44-74905efd2273	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	335e115b-8f25-4cad-96cf-22481c98c525	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	335e115b-8f25-4cad-96cf-22481c98c525	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	335e115b-8f25-4cad-96cf-22481c98c525	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	335e115b-8f25-4cad-96cf-22481c98c525	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	42a3ffaa-da90-44d8-ae9b-cfcb7a7bdac6	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	42a3ffaa-da90-44d8-ae9b-cfcb7a7bdac6	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	42a3ffaa-da90-44d8-ae9b-cfcb7a7bdac6	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	42a3ffaa-da90-44d8-ae9b-cfcb7a7bdac6	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	b0b538ba-3535-4308-8012-9e2fa0daa0b0	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	b0b538ba-3535-4308-8012-9e2fa0daa0b0	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	b0b538ba-3535-4308-8012-9e2fa0daa0b0	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	b0b538ba-3535-4308-8012-9e2fa0daa0b0	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	5a4b0eab-0a7b-463a-bcf8-0cca0fbddded	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	5a4b0eab-0a7b-463a-bcf8-0cca0fbddded	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	5a4b0eab-0a7b-463a-bcf8-0cca0fbddded	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	5a4b0eab-0a7b-463a-bcf8-0cca0fbddded	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	5a114aab-28cc-4655-b676-d08075bae831	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	5a114aab-28cc-4655-b676-d08075bae831	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	5a114aab-28cc-4655-b676-d08075bae831	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	5a114aab-28cc-4655-b676-d08075bae831	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	6a593127-4efb-4d7e-be38-53894c4828d0	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	6a593127-4efb-4d7e-be38-53894c4828d0	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	6a593127-4efb-4d7e-be38-53894c4828d0	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	6a593127-4efb-4d7e-be38-53894c4828d0	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	f87b0496-d9cb-4aef-b9dd-b7fe62ac3a18	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	f87b0496-d9cb-4aef-b9dd-b7fe62ac3a18	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	f87b0496-d9cb-4aef-b9dd-b7fe62ac3a18	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	f87b0496-d9cb-4aef-b9dd-b7fe62ac3a18	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	942cf428-3e36-48d2-8932-7e55cde20b32	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	942cf428-3e36-48d2-8932-7e55cde20b32	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	942cf428-3e36-48d2-8932-7e55cde20b32	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	942cf428-3e36-48d2-8932-7e55cde20b32	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	5b99b5c4-91a8-4492-80a2-71cd9b800f00	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	5b99b5c4-91a8-4492-80a2-71cd9b800f00	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	5b99b5c4-91a8-4492-80a2-71cd9b800f00	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	5b99b5c4-91a8-4492-80a2-71cd9b800f00	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	01803718-15d1-4ef4-9574-b9bb161c1638	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	01803718-15d1-4ef4-9574-b9bb161c1638	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	01803718-15d1-4ef4-9574-b9bb161c1638	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	01803718-15d1-4ef4-9574-b9bb161c1638	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	56bd7e72-5e17-4105-bf4d-5f698e94a7fb	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	56bd7e72-5e17-4105-bf4d-5f698e94a7fb	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	56bd7e72-5e17-4105-bf4d-5f698e94a7fb	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	56bd7e72-5e17-4105-bf4d-5f698e94a7fb	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	6f13d6e5-4322-4b4b-8fca-2278b04bd4eb	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	6f13d6e5-4322-4b4b-8fca-2278b04bd4eb	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	6f13d6e5-4322-4b4b-8fca-2278b04bd4eb	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	6f13d6e5-4322-4b4b-8fca-2278b04bd4eb	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	d62d3352-56e7-4802-973e-32d590febdab	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	d62d3352-56e7-4802-973e-32d590febdab	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	d62d3352-56e7-4802-973e-32d590febdab	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	d62d3352-56e7-4802-973e-32d590febdab	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	986182a1-8b33-457b-9151-38676f0f0869	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	986182a1-8b33-457b-9151-38676f0f0869	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	986182a1-8b33-457b-9151-38676f0f0869	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	986182a1-8b33-457b-9151-38676f0f0869	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	5c336652-4465-4b62-8de1-dac26fc696b6	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	5c336652-4465-4b62-8de1-dac26fc696b6	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	5c336652-4465-4b62-8de1-dac26fc696b6	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	5c336652-4465-4b62-8de1-dac26fc696b6	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	e9e7ff66-d302-49a3-a9e5-8a02ff87e016	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	e9e7ff66-d302-49a3-a9e5-8a02ff87e016	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	e9e7ff66-d302-49a3-a9e5-8a02ff87e016	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	e9e7ff66-d302-49a3-a9e5-8a02ff87e016	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	8c46f173-f586-402d-b54a-ac2e4ba57f1e	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	8c46f173-f586-402d-b54a-ac2e4ba57f1e	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	8c46f173-f586-402d-b54a-ac2e4ba57f1e	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	8c46f173-f586-402d-b54a-ac2e4ba57f1e	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	36aabcfa-60bc-48f5-9af6-30aa7600eb5b	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	0702e90c-cb7c-42a4-a447-478fba5a7443	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	0702e90c-cb7c-42a4-a447-478fba5a7443	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	0702e90c-cb7c-42a4-a447-478fba5a7443	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	0702e90c-cb7c-42a4-a447-478fba5a7443	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	e8f83478-a577-4cff-a06f-8d921f9367c7	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	e8f83478-a577-4cff-a06f-8d921f9367c7	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	e8f83478-a577-4cff-a06f-8d921f9367c7	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	e8f83478-a577-4cff-a06f-8d921f9367c7	3	t
29656af0-c3f6-44e8-9c74-c3643f38e871	8a8c6d23-963b-4819-819d-b9cdeaad7718	0	t
29656af0-c3f6-44e8-9c74-c3643f38e871	8a8c6d23-963b-4819-819d-b9cdeaad7718	1	t
29656af0-c3f6-44e8-9c74-c3643f38e871	8a8c6d23-963b-4819-819d-b9cdeaad7718	2	t
29656af0-c3f6-44e8-9c74-c3643f38e871	8a8c6d23-963b-4819-819d-b9cdeaad7718	3	t
29656af0-c3f6-44e8-9c74-c3643f38e871	8a0ba72f-67d2-481f-9e10-a188f09effa5	0	t
29656af0-c3f6-44e8-9c74-c3643f38e871	8a0ba72f-67d2-481f-9e10-a188f09effa5	1	t
29656af0-c3f6-44e8-9c74-c3643f38e871	8a0ba72f-67d2-481f-9e10-a188f09effa5	2	t
29656af0-c3f6-44e8-9c74-c3643f38e871	8a0ba72f-67d2-481f-9e10-a188f09effa5	3	t
29656af0-c3f6-44e8-9c74-c3643f38e871	95e9e230-c4f3-4fbc-9652-78cf4155d7ea	0	t
29656af0-c3f6-44e8-9c74-c3643f38e871	95e9e230-c4f3-4fbc-9652-78cf4155d7ea	1	t
29656af0-c3f6-44e8-9c74-c3643f38e871	95e9e230-c4f3-4fbc-9652-78cf4155d7ea	2	t
29656af0-c3f6-44e8-9c74-c3643f38e871	95e9e230-c4f3-4fbc-9652-78cf4155d7ea	3	t
29656af0-c3f6-44e8-9c74-c3643f38e871	fd48562f-9096-4cec-ad9c-37229fc072a3	0	t
29656af0-c3f6-44e8-9c74-c3643f38e871	fd48562f-9096-4cec-ad9c-37229fc072a3	1	t
29656af0-c3f6-44e8-9c74-c3643f38e871	fd48562f-9096-4cec-ad9c-37229fc072a3	2	t
29656af0-c3f6-44e8-9c74-c3643f38e871	fd48562f-9096-4cec-ad9c-37229fc072a3	3	t
29656af0-c3f6-44e8-9c74-c3643f38e871	5ab9c82d-a116-4032-8891-cbfb7b71b8e3	0	t
29656af0-c3f6-44e8-9c74-c3643f38e871	5ab9c82d-a116-4032-8891-cbfb7b71b8e3	1	t
29656af0-c3f6-44e8-9c74-c3643f38e871	5ab9c82d-a116-4032-8891-cbfb7b71b8e3	2	t
29656af0-c3f6-44e8-9c74-c3643f38e871	5ab9c82d-a116-4032-8891-cbfb7b71b8e3	3	t
29656af0-c3f6-44e8-9c74-c3643f38e871	e7be0d85-9f96-4095-be35-1da049028cef	0	t
29656af0-c3f6-44e8-9c74-c3643f38e871	e7be0d85-9f96-4095-be35-1da049028cef	1	t
29656af0-c3f6-44e8-9c74-c3643f38e871	e7be0d85-9f96-4095-be35-1da049028cef	2	t
29656af0-c3f6-44e8-9c74-c3643f38e871	e7be0d85-9f96-4095-be35-1da049028cef	3	t
29656af0-c3f6-44e8-9c74-c3643f38e871	b7ade8cc-22aa-43c8-be9c-af6cb71d11a6	0	t
29656af0-c3f6-44e8-9c74-c3643f38e871	b7ade8cc-22aa-43c8-be9c-af6cb71d11a6	1	t
29656af0-c3f6-44e8-9c74-c3643f38e871	b7ade8cc-22aa-43c8-be9c-af6cb71d11a6	2	t
29656af0-c3f6-44e8-9c74-c3643f38e871	b7ade8cc-22aa-43c8-be9c-af6cb71d11a6	3	t
29656af0-c3f6-44e8-9c74-c3643f38e871	b52e8eac-3bf6-4ebf-95a0-46ab9e7b0888	0	t
29656af0-c3f6-44e8-9c74-c3643f38e871	b52e8eac-3bf6-4ebf-95a0-46ab9e7b0888	1	t
29656af0-c3f6-44e8-9c74-c3643f38e871	b52e8eac-3bf6-4ebf-95a0-46ab9e7b0888	2	t
29656af0-c3f6-44e8-9c74-c3643f38e871	b52e8eac-3bf6-4ebf-95a0-46ab9e7b0888	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	f18fbb6e-bd6f-5d21-fa8d-11923327b436	0	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	a6043b21-18d0-4fcd-9ea9-f146542081d5	2	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	a6043b21-18d0-4fcd-9ea9-f146542081d5	3	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	084488a3-092d-4e8c-8bf2-72dcf90262b4	0	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	084488a3-092d-4e8c-8bf2-72dcf90262b4	1	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	084488a3-092d-4e8c-8bf2-72dcf90262b4	2	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	084488a3-092d-4e8c-8bf2-72dcf90262b4	3	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	870ec1d3-5b71-47dc-b241-1b0ae933217c	0	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	870ec1d3-5b71-47dc-b241-1b0ae933217c	1	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	870ec1d3-5b71-47dc-b241-1b0ae933217c	2	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	870ec1d3-5b71-47dc-b241-1b0ae933217c	3	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	b4be7b7c-4587-4af4-af07-fce34df723df	0	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	b4be7b7c-4587-4af4-af07-fce34df723df	2	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	b4be7b7c-4587-4af4-af07-fce34df723df	3	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	99302348-4d3c-48dd-8d67-c422e3061f1c	0	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	99302348-4d3c-48dd-8d67-c422e3061f1c	1	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	99302348-4d3c-48dd-8d67-c422e3061f1c	2	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	99302348-4d3c-48dd-8d67-c422e3061f1c	3	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	a6043b21-18d0-4fcd-9ea9-f146542081d5	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	948af0c2-5c0d-4887-8d81-4cd42e1b02a0	0	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	e8f83478-a577-4cff-a06f-8d921f9367c7	0	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	8a0ba72f-67d2-481f-9e10-a188f09effa5	0	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	8a0ba72f-67d2-481f-9e10-a188f09effa5	1	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	8a0ba72f-67d2-481f-9e10-a188f09effa5	2	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	8a0ba72f-67d2-481f-9e10-a188f09effa5	3	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	95e9e230-c4f3-4fbc-9652-78cf4155d7ea	0	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	95e9e230-c4f3-4fbc-9652-78cf4155d7ea	1	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	95e9e230-c4f3-4fbc-9652-78cf4155d7ea	2	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	5ab9c82d-a116-4032-8891-cbfb7b71b8e3	3	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	e7be0d85-9f96-4095-be35-1da049028cef	0	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	e7be0d85-9f96-4095-be35-1da049028cef	1	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	e7be0d85-9f96-4095-be35-1da049028cef	2	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	e7be0d85-9f96-4095-be35-1da049028cef	3	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	b7ade8cc-22aa-43c8-be9c-af6cb71d11a6	0	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	b7ade8cc-22aa-43c8-be9c-af6cb71d11a6	1	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	b7ade8cc-22aa-43c8-be9c-af6cb71d11a6	2	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	b7ade8cc-22aa-43c8-be9c-af6cb71d11a6	3	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	b52e8eac-3bf6-4ebf-95a0-46ab9e7b0888	0	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	b52e8eac-3bf6-4ebf-95a0-46ab9e7b0888	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	0a516b56-ed73-dafe-aaa0-332fadd2f088	2	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	e8f83478-a577-4cff-a06f-8d921f9367c7	1	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	e8f83478-a577-4cff-a06f-8d921f9367c7	2	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	e8f83478-a577-4cff-a06f-8d921f9367c7	3	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	295f6ddd-b934-4215-8e44-74905efd2273	0	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	335e115b-8f25-4cad-96cf-22481c98c525	0	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	d62d3352-56e7-4802-973e-32d590febdab	0	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	986182a1-8b33-457b-9151-38676f0f0869	0	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	5c336652-4465-4b62-8de1-dac26fc696b6	0	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	e9e7ff66-d302-49a3-a9e5-8a02ff87e016	0	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	42a3ffaa-da90-44d8-ae9b-cfcb7a7bdac6	0	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	b0b538ba-3535-4308-8012-9e2fa0daa0b0	0	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	5a4b0eab-0a7b-463a-bcf8-0cca0fbddded	0	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	6a593127-4efb-4d7e-be38-53894c4828d0	0	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	a6043b21-18d0-4fcd-9ea9-f146542081d5	0	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	b4be7b7c-4587-4af4-af07-fce34df723df	1	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	576b9f00-c29d-4c2c-9a6b-5a563344de93	0	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	576b9f00-c29d-4c2c-9a6b-5a563344de93	1	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	576b9f00-c29d-4c2c-9a6b-5a563344de93	2	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	576b9f00-c29d-4c2c-9a6b-5a563344de93	3	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	f87b0496-d9cb-4aef-b9dd-b7fe62ac3a18	0	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	942cf428-3e36-48d2-8932-7e55cde20b32	0	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	5b99b5c4-91a8-4492-80a2-71cd9b800f00	0	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	01803718-15d1-4ef4-9574-b9bb161c1638	0	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	36aabcfa-60bc-48f5-9af6-30aa7600eb5b	2	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	b52e8eac-3bf6-4ebf-95a0-46ab9e7b0888	2	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	b52e8eac-3bf6-4ebf-95a0-46ab9e7b0888	3	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	0702e90c-cb7c-42a4-a447-478fba5a7443	0	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	0702e90c-cb7c-42a4-a447-478fba5a7443	1	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	56bd7e72-5e17-4105-bf4d-5f698e94a7fb	0	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	8a8c6d23-963b-4819-819d-b9cdeaad7718	3	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	95e9e230-c4f3-4fbc-9652-78cf4155d7ea	3	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	fd48562f-9096-4cec-ad9c-37229fc072a3	0	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	fd48562f-9096-4cec-ad9c-37229fc072a3	1	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	fd48562f-9096-4cec-ad9c-37229fc072a3	2	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	fd48562f-9096-4cec-ad9c-37229fc072a3	3	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	5ab9c82d-a116-4032-8891-cbfb7b71b8e3	0	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	5ab9c82d-a116-4032-8891-cbfb7b71b8e3	1	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	5ab9c82d-a116-4032-8891-cbfb7b71b8e3	2	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	8a8c6d23-963b-4819-819d-b9cdeaad7718	1	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	0702e90c-cb7c-42a4-a447-478fba5a7443	2	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	0702e90c-cb7c-42a4-a447-478fba5a7443	3	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	8c46f173-f586-402d-b54a-ac2e4ba57f1e	0	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	8a8c6d23-963b-4819-819d-b9cdeaad7718	2	f
11dc1faf-2c66-4525-932d-a90e24da8987	f18fbb6e-bd6f-5d21-fa8d-11923327b436	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	f18fbb6e-bd6f-5d21-fa8d-11923327b436	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	f18fbb6e-bd6f-5d21-fa8d-11923327b436	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	a0c6daf8-b0c8-4e9d-9702-7a2bb781580c	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	a0c6daf8-b0c8-4e9d-9702-7a2bb781580c	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	a0c6daf8-b0c8-4e9d-9702-7a2bb781580c	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	a0c6daf8-b0c8-4e9d-9702-7a2bb781580c	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	b94a2365-c063-491e-a798-c68dccd2d80b	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	b94a2365-c063-491e-a798-c68dccd2d80b	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	b94a2365-c063-491e-a798-c68dccd2d80b	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	b94a2365-c063-491e-a798-c68dccd2d80b	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	a1b976fc-99d3-4b5f-89b5-bc7e7fc4c0d2	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	a1b976fc-99d3-4b5f-89b5-bc7e7fc4c0d2	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	a1b976fc-99d3-4b5f-89b5-bc7e7fc4c0d2	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	a1b976fc-99d3-4b5f-89b5-bc7e7fc4c0d2	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	7c7a2763-ed8b-41a7-a42d-b79233d02e02	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	7c7a2763-ed8b-41a7-a42d-b79233d02e02	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	7c7a2763-ed8b-41a7-a42d-b79233d02e02	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	7c7a2763-ed8b-41a7-a42d-b79233d02e02	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	33d081b5-8e4d-424b-a00a-eccf1f1a8809	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	33d081b5-8e4d-424b-a00a-eccf1f1a8809	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	33d081b5-8e4d-424b-a00a-eccf1f1a8809	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	33d081b5-8e4d-424b-a00a-eccf1f1a8809	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	65afc8bf-4df2-486a-b878-e77638ae2688	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	65afc8bf-4df2-486a-b878-e77638ae2688	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	65afc8bf-4df2-486a-b878-e77638ae2688	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	65afc8bf-4df2-486a-b878-e77638ae2688	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	948af0c2-5c0d-4887-8d81-4cd42e1b02a0	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	948af0c2-5c0d-4887-8d81-4cd42e1b02a0	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	948af0c2-5c0d-4887-8d81-4cd42e1b02a0	3	t
11dc1faf-2c66-4525-932d-a90e24da8987	fa7e83ee-9b49-4cda-badd-d68cda7b7a9a	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	f18fbb6e-bd6f-5d21-fa8d-11923327b436	0	f
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	f18fbb6e-bd6f-5d21-fa8d-11923327b436	1	f
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	f18fbb6e-bd6f-5d21-fa8d-11923327b436	2	f
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	f18fbb6e-bd6f-5d21-fa8d-11923327b436	3	f
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	8a8c6d23-963b-4819-819d-b9cdeaad7718	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	fa7e83ee-9b49-4cda-badd-d68cda7b7a9a	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	fd48562f-9096-4cec-ad9c-37229fc072a3	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	7c7a2763-ed8b-41a7-a42d-b79233d02e02	0	f
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	7c7a2763-ed8b-41a7-a42d-b79233d02e02	1	f
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	7c7a2763-ed8b-41a7-a42d-b79233d02e02	2	f
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	7c7a2763-ed8b-41a7-a42d-b79233d02e02	3	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	f18fbb6e-bd6f-5d21-fa8d-11923327b436	0	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	f18fbb6e-bd6f-5d21-fa8d-11923327b436	1	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	f18fbb6e-bd6f-5d21-fa8d-11923327b436	2	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	f18fbb6e-bd6f-5d21-fa8d-11923327b436	3	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	8a8c6d23-963b-4819-819d-b9cdeaad7718	0	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	fa7e83ee-9b49-4cda-badd-d68cda7b7a9a	0	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	7c7a2763-ed8b-41a7-a42d-b79233d02e02	0	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	7c7a2763-ed8b-41a7-a42d-b79233d02e02	1	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	7c7a2763-ed8b-41a7-a42d-b79233d02e02	2	f
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	7c7a2763-ed8b-41a7-a42d-b79233d02e02	3	f
11dc1faf-2c66-4525-932d-a90e24da8987	ed926af3-61a5-40e7-8975-de78c90eb784	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	ed926af3-61a5-40e7-8975-de78c90eb784	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	ed926af3-61a5-40e7-8975-de78c90eb784	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	ed926af3-61a5-40e7-8975-de78c90eb784	3	t
\.



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
-- Data for Name: m_user; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY m_user(user_id, role_id, name_user, user_password, is_active, status_user, email) FROM stdin;
577b035d-64d5-427c-80c5-79c0fe1ae074	c58ee40a-5ae2-4067-b6ad-8cae9c65913c	rudi	b79f6fe35c28cbb72373b30f726141c7	t	2	\N
98281a7b-4d74-4e85-a215-0f8740795588	42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	kasir	b79f6fe35c28cbb72373b30f726141c7	t	2	\N
50c6d67c-d9dd-462a-a118-75ed0b185b14	11dc1faf-2c66-4525-932d-a90e24da8987	aaa	b79f6fe35c28cbb72373b30f726141c7	t	2	admin@gmail.com
00b5acfa-b533-454b-8dfd-e7881edd180f	11dc1faf-2c66-4525-932d-a90e24da8987	admin	74521f341a6473f2bea7fa0ef052e7a8	t	2	\N
72ab191f-8b04-4b16-9931-930ef70b670c	11dc1faf-2c66-4525-932d-a90e24da8987	xyz	a03180fabf135452f17a7e81e33860fe	t	2	\N
\.


--
-- PostgreSQL database dump complete
--


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
-- Data for Name: m_database_version; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY m_database_version (version_number) FROM stdin;
12
\.


--
-- PostgreSQL database dump complete
--


--
-- PostgreSQL database dump
--
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
-- Data for Name: m_expense_type; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY m_expense_type (expense_type_id, name_expense_type) FROM stdin;
26b062ad-7469-42c4-9326-bb84feeca746	Electricity Cost
6c262064-6453-4bea-9e0f-5ae1810d0557	Advertisement Cost
cd381cd3-dd95-46c3-aff4-d36d9ae02faa	Office Supplies Cost
b7968f37-5a92-4ea3-bff0-2909aed18d9d	Office Equipment Depreciation Cost
184f087f-8bd6-4b2c-9c53-16aeefa1a346	Building Rent Cost
2cc2ae56-dc3b-4991-af56-7768ae10816a	Marketing SPJ Cost
c2116c49-a940-4385-be94-302470b67b83	Vehicle Depreciation Cost
b0188e31-01cd-4825-9ab8-e1bb433e15df	Web API Development Cost
2d921654-2646-4e38-b09c-d691a40469b4	Office Stationery Cost
1619f95e-eac3-40e9-ba8a-af2230d3c470	Other Sales Costs
a8edb7ee-7807-4dfd-afc0-9ddc6006ca36	Other Costs
\.

--
-- PostgreSQL database dump complete
--

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
-- Data for Name: m_job_titles; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY m_job_titles (job_titles_id, name_job_titles, description) FROM stdin;
120d3472-ea93-4e29-8abd-5bd7044d26db	Kasir	
f1e4ea09-b777-4e56-bb90-db2bf9211468	General Manager	
583b27e0-1644-4884-8651-47789e7713e5	Staff Administrator	
edb47227-da98-4d97-bff2-b7ee41ff3400	Owner	Store owner
955d42f3-bd82-4aa7-8c9f-8a6207b0494d	Security	
def6e55c-47d4-4381-9a67-4f2cdce1db4d	Driver	valueDiscription
\.




--
-- PostgreSQL database dump complete
--

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
-- Data for Name: m_profile; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY m_profile (profile_id, name_profile, address, city, phone, email, website, register_id, is_register, hash) FROM stdin;
ced140f5-52f5-4617-b8c0-ebb5cae2c769	KR Software	Jl. Raya Berbah	Yogyakarta	0813 8176 9915	rudi.krsoftware@gmail.com	https://github.com/rudi-krsoftware/open-retail/	\N	f	\N
\.


--
-- PostgreSQL database dump complete
--


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
-- Data for Name: m_invoice_header; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY m_invoice_header (header_invoice_id, description, order_number, is_active) FROM stdin;
fdc91c63-7497-ae8c-36ba-2e6762a16408	KR Software - https://github.com/rudi-krsoftware/open-retail/	1	t
3e4dca2d-7041-daa9-5d32-dfc4f68cd306	Jl. Raya Berbah	2	t
567abd02-cd14-1c97-262b-805038b7d6a8	Berbah, Yogyakarta	3	t
3ed349af-4671-a8ce-6ff4-6467556343e4	HP: 0813 8176 9915, Email: rudi.krsoftware@gmail.com	4	t
b01bf51a-d03c-1e70-256c-4e5384eaa782	Rek: MANDIRI 137-00-0553-7820 MUAMALAT 537-000-1068 a.n Kamaruddin	5	t
\.


--
-- PostgreSQL database dump complete
--



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
-- Data for Name: m_invoice_label; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY m_invoice_label (invoice_label_id, description, order_number, is_active) FROM stdin;
04fb83d6-f9e8-3dbc-0b1b-a8d3b0b0e765	KR Software	1	t
dfda864a-0019-5386-2ead-8f0f3686eaa4	Jl. Raya Berbah	2	t
add6061c-39be-238b-8c7a-7248f93ca8b6	HP: 0813 8176 9915	3	t
\.


--
-- PostgreSQL database dump complete
--


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
-- Data for Name: m_province; Type: TABLE DATA; Schema: public; Owner: postgres
--
COPY m_province (province_id, name_province) FROM stdin;
1	Bali
2	Bangka Belitung
3	Banten
4	Bengkulu
5	DI Yogyakarta
6	DKI Jakarta
7	Gorontalo
8	timebi
9	Jawa Barat
10	Jawa Tengah
11	Jawa Timur
12	Kalimantan Barat
13	Kalimantan Selatan
14	Kalimantan Tengah
15	Kalimantan Timur
16	Kalimantan Utara
17	Kepulauan Riau
18	Lampung
19	Maluku
20	Maluku Utara
21	Nanggroe Aceh Darussalam (NAD)
22	Nusa Tenggara Barat (NTB)
23	Nusa Tenggara Timur (NTT)
24	Papua
25	Papua Barat
26	Riau
27	Sulawesi Barat
28	Sulawesi Selatan
29	Sulawesi Tengah
30	Sulawesi Tenggara
31	Sulawesi Utara
32	Sumatera Barat
33	Sumatera Selatan
34	Sumatera Utara
\.




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
-- Data for Name: m_regency; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY m_regency (regency_id, province_id, type, name_regency, postal_code) FROM stdin;
1	21	regency	Aceh Barat	23681
2	21	regency	Aceh Barat Daya	23764
3	21	regency	Aceh Besar	23951
4	21	regency	Aceh Jaya	23654
5	21	regency	Aceh Selatan	23719
6	21	regency	Aceh Singkil	24785
7	21	regency	Aceh Tamiang	24476
8	21	regency	Aceh Tengah	24511
9	21	regency	Aceh Tenggara	24611
10	21	regency	Aceh Timur	24454
11	21	regency	Aceh Utara	24382
12	32	regency	Agam	26411
13	23	regency	Alor	85811
14	19	city	Ambon	97222
15	34	regency	Asahan	21214
16	24	regency	Asmat	99777
17	1	regency	Badung	80351
18	13	regency	Balangan	71611
19	15	city	Balikpapan	76111
20	21	city	Banda Aceh	23238
21	18	city	Bandar Lampung	35139
22	9	regency	Bandung	40311
23	9	city	Bandung	40115
24	9	regency	Bandung Barat	40721
25	29	regency	Banggai	94711
26	29	regency	Banggai Kepulauan	94881
27	2	regency	Bangka	33212
28	2	regency	Bangka Barat	33315
29	2	regency	Bangka Selatan	33719
30	2	regency	Bangka Tengah	33613
31	11	regency	Bangkalan	69118
32	1	regency	Bangli	80619
33	13	regency	Banjar	70619
34	9	city	Banjar	46311
35	13	city	Banjarbaru	70712
36	13	city	Banjarmasin	70117
37	10	regency	Banjarnegara	53419
38	28	regency	Bantaeng	92411
39	5	regency	Bantul	55715
40	33	regency	Banyuasin	30911
41	10	regency	Banyumas	53114
42	11	regency	Banyuwangi	68416
43	13	regency	Barito Kuala	70511
44	14	regency	Barito Selatan	73711
45	14	regency	Barito Timur	73671
46	14	regency	Barito Utara	73881
47	28	regency	Barru	90719
48	17	city	Batam	29413
49	10	regency	Batang	51211
50	8	regency	Batang Hari	36613
51	11	city	Batu	65311
52	34	regency	Batu Bara	21655
53	30	city	Bau-Bau	93719
54	9	regency	Bekasi	17837
55	9	city	Bekasi	17121
56	2	regency	Belitung	33419
57	2	regency	Belitung Timur	33519
58	23	regency	Belu	85711
59	21	regency	Bener Meriah	24581
60	26	regency	Bengkalis	28719
61	12	regency	Bengkayang	79213
62	4	city	Bengkulu	38229
63	4	regency	Bengkulu Selatan	38519
64	4	regency	Bengkulu Tengah	38319
65	4	regency	Bengkulu Utara	38619
66	15	regency	Berau	77311
67	24	regency	Biak Numfor	98119
68	22	regency	Bima	84171
69	22	city	Bima	84139
70	34	city	Binjai	20712
71	17	regency	Bintan	29135
72	21	regency	Bireuen	24219
73	31	city	Bitung	95512
74	11	regency	Blitar	66171
75	11	city	Blitar	66124
76	10	regency	Blora	58219
77	7	regency	Boalemo	96319
78	9	regency	Bogor	16911
79	9	city	Bogor	16119
80	11	regency	Bojonegoro	62119
81	31	regency	Bolaang Mongondow (Bolmong)	95755
82	31	regency	Bolaang Mongondow Selatan	95774
83	31	regency	Bolaang Mongondow Timur	95783
84	31	regency	Bolaang Mongondow Utara	95765
85	30	regency	Bombana	93771
86	11	regency	Bondowoso	68219
87	28	regency	Bone	92713
88	7	regency	Bone Bolango	96511
89	15	city	Bontang	75313
90	24	regency	Boven Digoel	99662
91	10	regency	Boyolali	57312
92	10	regency	Brebes	52212
93	32	city	Bukittinggi	26115
94	1	regency	Buleleng	81111
95	28	regency	Bulukumba	92511
96	16	regency	Bulungan (Bulongan)	77211
97	8	regency	Bungo	37216
98	29	regency	Buol	94564
99	19	regency	Buru	97371
100	19	regency	Buru Selatan	97351
101	30	regency	Buton	93754
102	30	regency	Buton Utara	93745
103	9	regency	Ciamis	46211
104	9	regency	Cianjur	43217
105	10	regency	Cilacap	53211
106	3	city	Cilegon	42417
107	9	city	Cimahi	40512
108	9	regency	Cirebon	45611
109	9	city	Cirebon	45116
110	34	regency	Dairi	22211
111	24	regency	Deiyai (Deliyai)	98784
112	34	regency	Deli Serdang	20511
113	10	regency	Demak	59519
114	1	city	Denpasar	80227
115	9	city	Depok	16416
116	32	regency	Dharmasraya	27612
117	24	regency	Dogiyai	98866
118	22	regency	Dompu	84217
119	29	regency	Donggala	94341
120	26	city	Dumai	28811
121	33	regency	Empat Lawang	31811
122	23	regency	Ende	86351
123	28	regency	Enrekang	91719
124	25	regency	Fakfak	98651
125	23	regency	Flores Timur	86213
126	9	regency	Garut	44126
127	21	regency	Gayo Lues	24653
128	1	regency	Gianyar	80519
129	7	regency	Gorontalo	96218
130	7	city	Gorontalo	96115
131	7	regency	Gorontalo Utara	96611
132	28	regency	Gowa	92111
133	11	regency	Gresik	61115
134	10	regency	Grobogan	58111
135	5	regency	Gunung Kidul	55812
136	14	regency	Gunung Mas	74511
137	34	city	Gunungsitoli	22813
138	20	regency	Halmahera Barat	97757
139	20	regency	Halmahera Selatan	97911
140	20	regency	Halmahera Tengah	97853
141	20	regency	Halmahera Timur	97862
142	20	regency	Halmahera Utara	97762
143	13	regency	Hulu Sungai Selatan	71212
144	13	regency	Hulu Sungai Tengah	71313
145	13	regency	Hulu Sungai Utara	71419
146	34	regency	Humbang Hasundutan	22457
147	26	regency	Indragiri Hilir	29212
148	26	regency	Indragiri Hulu	29319
149	9	regency	Indramayu	45214
150	24	regency	Intan Jaya	98771
151	6	city	Jakarta Barat	11220
152	6	city	Jakarta Pusat	10540
153	6	city	Jakarta Selatan	12230
154	6	city	Jakarta Timur	13330
155	6	city	Jakarta Utara	14140
156	8	city	timebi	36111
157	24	regency	Jayapura	99352
158	24	city	Jayapura	99114
159	24	regency	Jayawijaya	99511
160	11	regency	Jember	68113
161	1	regency	Jembrana	82251
162	28	regency	Jeneponto	92319
163	10	regency	Jepara	59419
164	11	regency	Jombang	61415
165	25	regency	Kaimana	98671
166	26	regency	Kampar	28411
167	14	regency	Kapuas	73583
168	12	regency	Kapuas Hulu	78719
169	10	regency	Karanganyar	57718
170	1	regency	Karangasem	80819
171	9	regency	Karawang	41311
172	17	regency	Karimun	29611
173	34	regency	Karo	22119
174	14	regency	Katingan	74411
175	4	regency	Kaur	38911
176	12	regency	Kayong Utara	78852
177	10	regency	Kebumen	54319
178	11	regency	Kediri	64184
179	11	city	Kediri	64125
180	24	regency	Keerom	99461
181	10	regency	Kendal	51314
182	30	city	Kendari	93126
183	4	regency	Kepahiang	39319
184	17	regency	Kepulauan Anambas	29991
185	19	regency	Kepulauan Aru	97681
186	32	regency	Kepulauan Mentawai	25771
187	26	regency	Kepulauan Meranti	28791
188	31	regency	Kepulauan Sangihe	95819
189	6	regency	Kepulauan Seribu	14550
190	31	regency	Kepulauan Siau Tagulandang Biaro (Sitaro)	95862
191	20	regency	Kepulauan Sula	97995
192	31	regency	Kepulauan Talaud	95885
193	24	regency	Kepulauan Yapen (Yapen Waropen)	98211
194	8	regency	Kerinci	37167
195	12	regency	Ketapang	78874
196	10	regency	Klaten	57411
197	1	regency	Klungkung	80719
198	30	regency	Kolaka	93511
199	30	regency	Kolaka Utara	93911
200	30	regency	Konawe	93411
201	30	regency	Konawe Selatan	93811
202	30	regency	Konawe Utara	93311
203	13	regency	citybaru	72119
204	31	city	citymobagu	95711
205	14	regency	citywaringin Barat	74119
206	14	regency	citywaringin Timur	74364
207	26	regency	Kuantan Singingi	29519
208	12	regency	Kubu Raya	78311
209	10	regency	Kudus	59311
210	5	regency	Kulon Progo	55611
211	9	regency	Kuningan	45511
212	23	regency	Kupang	85362
213	23	city	Kupang	85119
214	15	regency	Kutai Barat	75711
215	15	regency	Kutai Kartanegara	75511
216	15	regency	Kutai Timur	75611
217	34	regency	Labuhan Batu	21412
218	34	regency	Labuhan Batu Selatan	21511
219	34	regency	Labuhan Batu Utara	21711
220	33	regency	Lahat	31419
221	14	regency	Lamandau	74611
222	11	regency	Lamongan	64125
223	18	regency	Lampung Barat	34814
224	18	regency	Lampung Selatan	35511
225	18	regency	Lampung Tengah	34212
226	18	regency	Lampung Timur	34319
227	18	regency	Lampung Utara	34516
228	12	regency	Landak	78319
229	34	regency	Langkat	20811
230	21	city	Langsa	24412
231	24	regency	Lanny Jaya	99531
232	3	regency	Lebak	42319
233	4	regency	Lebong	39264
234	23	regency	Lembata	86611
235	21	city	Lhokseumawe	24352
236	32	regency	Lima Puluh Koto/city	26671
237	17	regency	Lingga	29811
238	22	regency	Lombok Barat	83311
239	22	regency	Lombok Tengah	83511
240	22	regency	Lombok Timur	83612
241	22	regency	Lombok Utara	83711
242	33	city	Lubuk Linggau	31614
243	11	regency	Lumajang	67319
244	28	regency	Luwu	91994
245	28	regency	Luwu Timur	92981
246	28	regency	Luwu Utara	92911
247	11	regency	Madiun	63153
248	11	city	Madiun	63122
249	10	regency	Magelang	56519
250	10	city	Magelang	56133
251	11	regency	Magetan	63314
252	9	regency	Majalengka	45412
253	27	regency	Majene	91411
254	28	city	Makassar	90111
255	11	regency	Malang	65163
256	11	city	Malang	65112
257	16	regency	Malinau	77511
258	19	regency	Maluku Barat Daya	97451
259	19	regency	Maluku Tengah	97513
260	19	regency	Maluku Tenggara	97651
261	19	regency	Maluku Tenggara Barat	97465
262	27	regency	Mamasa	91362
263	24	regency	Mamberamo Raya	99381
264	24	regency	Mamberamo Tengah	99553
265	27	regency	Mamuju	91519
266	27	regency	Mamuju Utara	91571
267	31	city	Manado	95247
268	34	regency	Mandailing Natal	22916
269	23	regency	Manggarai	86551
270	23	regency	Manggarai Barat	86711
271	23	regency	Manggarai Timur	86811
272	25	regency	Manokwari	98311
273	25	regency	Manokwari Selatan	98355
274	24	regency	Mappi	99853
275	28	regency	Maros	90511
276	22	city	Mataram	83131
277	25	regency	Maybrat	98051
278	34	city	Medan	20228
279	12	regency	Melawi	78619
280	8	regency	Merangin	37319
281	24	regency	Merauke	99613
282	18	regency	Mesuji	34911
283	18	city	Metro	34111
284	24	regency	Mimika	99962
285	31	regency	Minahasa	95614
286	31	regency	Minahasa Selatan	95914
287	31	regency	Minahasa Tenggara	95995
288	31	regency	Minahasa Utara	95316
289	11	regency	Mojokerto	61382
290	11	city	Mojokerto	61316
291	29	regency	Morowali	94911
292	33	regency	Muara Enim	31315
293	8	regency	Muaro timebi	36311
294	4	regency	Muko Muko	38715
295	30	regency	Muna	93611
296	14	regency	Murung Raya	73911
297	33	regency	Musi Banyuasin	30719
298	33	regency	Musi Rawas	31661
299	24	regency	Nabire	98816
300	21	regency	Nagan Raya	23674
301	23	regency	Nagekeo	86911
302	17	regency	Natuna	29711
303	24	regency	Nduga	99541
304	23	regency	Ngada	86413
305	11	regency	Nganjuk	64414
306	11	regency	Ngawi	63219
307	34	regency	Nias	22876
308	34	regency	Nias Barat	22895
309	34	regency	Nias Selatan	22865
310	34	regency	Nias Utara	22856
311	16	regency	Nunukan	77421
312	33	regency	Ogan Ilir	30811
313	33	regency	Ogan Komering Ilir	30618
314	33	regency	Ogan Komering Ulu	32112
315	33	regency	Ogan Komering Ulu Selatan	32211
316	33	regency	Ogan Komering Ulu Timur	32312
317	11	regency	Pacitan	63512
318	32	city	Padang	25112
319	34	regency	Padang Lawas	22763
320	34	regency	Padang Lawas Utara	22753
321	32	city	Padang Panjang	27122
322	32	regency	Padang Pariaman	25583
323	34	city	Padang Sidempuan	22727
324	33	city	Pagar Alam	31512
325	34	regency	Pakpak Bharat	22272
326	14	city	Palangka Raya	73112
327	33	city	Palembang	31512
328	28	city	Palopo	91911
329	29	city	Palu	94111
330	11	regency	Pamekasan	69319
331	3	regency	Pandeglang	42212
332	9	regency	Pangandaran	46511
333	28	regency	Pangkajene Kepulauan	90611
334	2	city	Pangkal Pinang	33115
335	24	regency	Paniai	98765
336	28	city	Parepare	91123
337	32	city	Pariaman	25511
338	29	regency	Parigi Moutong	94411
339	32	regency	Pasaman	26318
340	32	regency	Pasaman Barat	26511
341	15	regency	Paser	76211
342	11	regency	Pasuruan	67153
343	11	city	Pasuruan	67118
344	10	regency	Pati	59114
345	32	city	Payakumbuh	26213
346	25	regency	Pegunungan Arfak	98354
347	24	regency	Pegunungan Bintang	99573
348	10	regency	Pekalongan	51161
349	10	city	Pekalongan	51122
350	26	city	Pekanbaru	28112
351	26	regency	Pelalawan	28311
352	10	regency	Pemalang	52319
353	34	city	Pematang Siantar	21126
354	15	regency	Penatime Paser Utara	76311
355	18	regency	Pesawaran	35312
356	18	regency	Pesisir Barat	35974
357	32	regency	Pesisir Selatan	25611
358	21	regency	Pidie	24116
359	21	regency	Pidie Jaya	24186
360	28	regency	Pinrang	91251
361	7	regency	Pohuwato	96419
362	27	regency	Polewali Mandar	91311
363	11	regency	Ponorogo	63411
364	12	regency	Pontianak	78971
365	12	city	Pontianak	78112
366	29	regency	Poso	94615
367	33	city	Prabumulih	31121
368	18	regency	Pringsewu	35719
369	11	regency	Probolinggo	67282
370	11	city	Probolinggo	67215
371	14	regency	Pulang Pisau	74811
372	20	regency	Pulau Morotai	97771
373	24	regency	Puncak	98981
374	24	regency	Puncak Jaya	98979
375	10	regency	Purbalingga	53312
376	9	regency	Purwakarta	41119
377	10	regency	Purworejo	54111
378	25	regency	Raja Ampat	98489
379	4	regency	Rejang Lebong	39112
380	10	regency	Rembang	59219
381	26	regency	Rokan Hilir	28992
382	26	regency	Rokan Hulu	28511
383	23	regency	Rote Ndao	85982
384	21	city	Sabang	23512
385	23	regency	Sabu Raijua	85391
386	10	city	Salatiga	50711
387	15	city	Samarinda	75133
388	12	regency	Sambas	79453
389	34	regency	Samosir	22392
390	11	regency	Sampang	69219
391	12	regency	Sanggau	78557
392	24	regency	Sarmi	99373
393	8	regency	Sarolangun	37419
394	32	city	Sawah Lunto	27416
395	12	regency	Sekadau	79583
396	28	regency	Selayar (Kepulauan Selayar)	92812
397	4	regency	Seluma	38811
398	10	regency	Semarang	50511
399	10	city	Semarang	50135
400	19	regency	Seram Bagian Barat	97561
401	19	regency	Seram Bagian Timur	97581
402	3	regency	Serang	42182
403	3	city	Serang	42111
404	34	regency	Serdang Bedagai	20915
405	14	regency	Seruyan	74211
406	26	regency	Siak	28623
407	34	city	Sibolga	22522
408	28	regency	Sidenreng Rappang/Rapang	91613
409	11	regency	Sidoarjo	61219
410	29	regency	Sigi	94364
411	32	regency	Sijunjung (Sawah Lunto Sijunjung)	27511
412	23	regency	Sikka	86121
413	34	regency	Simalungun	21162
414	21	regency	Simeulue	23891
415	12	city	Singkawang	79117
416	28	regency	Sinjai	92615
417	12	regency	Sintang	78619
418	11	regency	Situbondo	68316
419	5	regency	Sleman	55513
420	32	regency	Solok	27365
421	32	city	Solok	27315
422	32	regency	Solok Selatan	27779
423	28	regency	Soppeng	90812
424	25	regency	Sorong	98431
425	25	city	Sorong	98411
426	25	regency	Sorong Selatan	98454
427	10	regency	Sragen	57211
428	9	regency	Subang	41215
429	21	city	Subulussalam	24882
430	9	regency	Sukabumi	43311
431	9	city	Sukabumi	43114
432	14	regency	Sukamara	74712
433	10	regency	Sukoharjo	57514
434	23	regency	Sumba Barat	87219
435	23	regency	Sumba Barat Daya	87453
436	23	regency	Sumba Tengah	87358
437	23	regency	Sumba Timur	87112
438	22	regency	Sumbawa	84315
439	22	regency	Sumbawa Barat	84419
440	9	regency	Sumedang	45326
441	11	regency	Sumenep	69413
442	8	city	Sungaipenuh	37113
443	24	regency	Supiori	98164
444	11	city	Surabaya	60119
445	10	city	Surakarta (Solo)	57113
446	13	regency	Tabalong	71513
447	1	regency	Tabanan	82119
448	28	regency	Takalar	92212
449	25	regency	Tambrauw	98475
450	16	regency	Tana Tidung	77611
451	28	regency	Tana Toraja	91819
452	13	regency	Tanah Bumbu	72211
453	32	regency	Tanah Datar	27211
454	13	regency	Tanah Laut	70811
455	3	regency	Tangerang	15914
456	3	city	Tangerang	15111
457	3	city	Tangerang Selatan	15332
458	18	regency	Tanggamus	35619
459	34	city	Tanjung Balai	21321
460	8	regency	Tanjung Jabung Barat	36513
461	8	regency	Tanjung Jabung Timur	36719
462	17	city	Tanjung Pinang	29111
463	34	regency	Tapanuli Selatan	22742
464	34	regency	Tapanuli Tengah	22611
465	34	regency	Tapanuli Utara	22414
466	13	regency	Tapin	71119
467	16	city	Tarakan	77114
468	9	regency	Tasikmalaya	46411
469	9	city	Tasikmalaya	46116
470	34	city	Tebing Tinggi	20632
471	8	regency	Tebo	37519
472	10	regency	Tegal	52419
473	10	city	Tegal	52114
474	25	regency	Teluk Bintuni	98551
475	25	regency	Teluk Wondama	98591
476	10	regency	Temanggung	56212
477	20	city	Ternate	97714
478	20	city	Tidore Kepulauan	97815
479	23	regency	Timor Tengah Selatan	85562
480	23	regency	Timor Tengah Utara	85612
481	34	regency	Toba Samosir	22316
482	29	regency	Tojo Una-Una	94683
483	29	regency	Toli-Toli	94542
484	24	regency	Tolikara	99411
485	31	city	Tomohon	95416
486	28	regency	Toraja Utara	91831
487	11	regency	Trenggalek	66312
488	19	city	Tual	97612
489	11	regency	Tuban	62319
490	18	regency	Tulang Bawang	34613
491	18	regency	Tulang Bawang Barat	34419
492	11	regency	Tulungagung	66212
493	28	regency	Wajo	90911
494	30	regency	Wakatobi	93791
495	24	regency	Waropen	98269
496	18	regency	Way Kanan	34711
497	10	regency	Wonogiri	57619
498	10	regency	Wonosobo	56311
499	24	regency	Yahukimo	99041
500	24	regency	Yalimo	99481
501	5	city	Yogyakarta	55222
\.


--
-- PostgreSQL database dump complete
--


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
-- Data for Name: m_invoice_header_mini_pos; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY m_invoice_header_mini_pos (header_invoice_id, description, order_number, is_active) FROM stdin;
2d51b16b-73c7-6d91-bdd9-099f76b342ad	KR Software	1	t
d131a867-9ce6-ab39-7fa0-5da0f1f4acc2	Jl. Raya Berbah	2	t
089d446a-fd39-cf69-7ce7-d734bf845a16	NPWP: 1234567890	3	t
8979e6c1-c3a4-b21a-5acb-38696d29e975	Berbah, Yogyakarta	4	t
57fd0c2a-450a-32c1-f4f1-1fec61a438cb	0813 8176 9915	5	t
\.


--
-- PostgreSQL database dump complete
--



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
-- Data for Name: m_mini_pos_invoice_footer; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY m_mini_pos_invoice_footer (footer_invoice_id, description, order_number, is_active) FROM stdin;
4c50cfc6-9cfb-0dcd-c648-76db95e6f1a4	TERIMA KASIH. SELAMAT BELANJA KEMBALI	1	t
63602003-1368-81e4-dd6c-0bc7b25417d7	=== LAYANAN KONSUMEN ===	2	t
82ae35a7-bc2b-f4c1-ea53-43c8386ddc56	SMS: 081381769915 CALL: 1102580	3	t
\.


--
-- PostgreSQL database dump complete
--


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
-- Data for Name: m_card; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY m_card (card_id, card_name, is_debit) FROM stdin;
23f39815-30ed-44a0-92ef-95bbebe86857	Debit BNI	t
4939cbfa-0eb2-4f43-89a8-58285573762f	Visa	f
eadb5ebe-aca8-44b3-aa28-34263507c8ad	Mandiri Debit Card	t
fbf224d1-d109-4280-a465-e3ff04494cc2	Mastercard	f
\.


--
-- PostgreSQL database dump complete
--


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
-- Data for Name: m_application_setting; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY m_application_setting (application_setting_id, is_update_iselling_price_master_produk, is_negative_stock_allowed_for_products, is_focus_on_inputting_quantity_column, is_tampilkan_description_tambahan_item_jual, description_tambahan_item_jual) FROM stdin;
1ef7cbcb-bf73-44be-aad2-b9d8e1b1a178	f	t	f	f	description
\.


--
-- PostgreSQL database dump complete
--



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
-- Data for Name: m_province2; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY m_province2 (province_id, name_province) FROM stdin;
11	Aceh
12	Sumatera Utara
13	Sumatera Barat
14	Riau
15	timebi
16	Sumatera Selatan
17	Bengkulu
18	Lampung
19	Kepulauan Bangka Belitung
21	Kepulauan Riau
32	Jawa Barat
33	Jawa Tengah
35	Jawa Timur
36	Banten
51	Bali
52	Nusa Tenggara Barat
53	Nusa Tenggara Timur
61	Kalimantan Barat
62	Kalimantan Tengah
63	Kalimantan Selatan
64	Kalimantan Timur
65	Kalimantan Utara
71	Sulawesi Utara
72	Sulawesi Tengah
73	Sulawesi Selatan
74	Sulawesi Tenggara
75	Gorontalo
76	Sulawesi Barat
81	Maluku
82	Maluku Utara
91	Papua Barat
94	Papua
34	DI Yogyakarta
31	DKI Jakarta
\.


--
-- PostgreSQL database dump complete
--


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
-- Data for Name: m_regency2; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY m_regency2 (regency_id, province_id, name_regency) FROM stdin;
1101	11	Kab. Simeulue
1102	11	Kab. Aceh Singkil
1103	11	Kab. Aceh Selatan
1104	11	Kab. Aceh Tenggara
1105	11	Kab. Aceh Timur
1106	11	Kab. Aceh Tengah
1107	11	Kab. Aceh Barat
1108	11	Kab. Aceh Besar
1109	11	Kab. Pidie
1110	11	Kab. Bireuen
1111	11	Kab. Aceh Utara
1112	11	Kab. Aceh Barat Daya
1113	11	Kab. Gayo Lues
1114	11	Kab. Aceh Tamiang
1115	11	Kab. Nagan Raya
1116	11	Kab. Aceh Jaya
1117	11	Kab. Bener Meriah
1118	11	Kab. Pidie Jaya
1171	11	city Banda Aceh
1172	11	city Sabang
1173	11	city Langsa
1174	11	city Lhokseumawe
1175	11	city Subulussalam
1201	12	Kab. Nias
1202	12	Kab. Mandailing Natal
1203	12	Kab. Tapanuli Selatan
1204	12	Kab. Tapanuli Tengah
1205	12	Kab. Tapanuli Utara
1206	12	Kab. Toba Samosir
1207	12	Kab. Labuhan Batu
1208	12	Kab. Asahan
1209	12	Kab. Simalungun
1210	12	Kab. Dairi
1211	12	Kab. Karo
1212	12	Kab. Deli Serdang
1213	12	Kab. Langkat
1214	12	Kab. Nias Selatan
1215	12	Kab. Humbang Hasundutan
1216	12	Kab. Pakpak Bharat
1217	12	Kab. Samosir
1218	12	Kab. Serdang Bedagai
1219	12	Kab. Batu Bara
1220	12	Kab. Padang Lawas Utara
1221	12	Kab. Padang Lawas
1222	12	Kab. Labuhan Batu Selatan
1223	12	Kab. Labuhan Batu Utara
1224	12	Kab. Nias Utara
1225	12	Kab. Nias Barat
1271	12	city Sibolga
1272	12	city Tanjung Balai
1273	12	city Pematang Siantar
1274	12	city Tebing Tinggi
1275	12	city Medan
1276	12	city Binjai
1277	12	city Padangsidimpuan
1278	12	city Gunungsitoli
1301	13	Kab. Kepulauan Mentawai
1302	13	Kab. Pesisir Selatan
1303	13	Kab. Solok
1304	13	Kab. Sijunjung
1305	13	Kab. Tanah Datar
1306	13	Kab. Padang Pariaman
1307	13	Kab. Agam
1308	13	Kab. Lima Puluh city
1309	13	Kab. Pasaman
1310	13	Kab. Solok Selatan
1311	13	Kab. Dharmasraya
1312	13	Kab. Pasaman Barat
1371	13	city Padang
1372	13	city Solok
1373	13	city Sawah Lunto
1374	13	city Padang Panjang
1375	13	city Bukittinggi
1376	13	city Payakumbuh
1377	13	city Pariaman
1401	14	Kab. Kuantan Singingi
1402	14	Kab. Indragiri Hulu
1403	14	Kab. Indragiri Hilir
1404	14	Kab. Pelalawan
1405	14	Kab. S I A K
1406	14	Kab. Kampar
1407	14	Kab. Rokan Hulu
1408	14	Kab. Bengkalis
1409	14	Kab. Rokan Hilir
1410	14	Kab. Kepulauan Meranti
1471	14	city Pekanbaru
1473	14	city D U M A I
1501	15	Kab. Kerinci
1502	15	Kab. Merangin
1503	15	Kab. Sarolangun
1504	15	Kab. Batang Hari
1505	15	Kab. Muaro timebi
1506	15	Kab. Tanjung Jabung Timur
1507	15	Kab. Tanjung Jabung Barat
1508	15	Kab. Tebo
1509	15	Kab. Bungo
1571	15	city timebi
1572	15	city Sungai Penuh
1601	16	Kab. Ogan Komering Ulu
1602	16	Kab. Ogan Komering Ilir
1603	16	Kab. Muara Enim
1604	16	Kab. Lahat
1605	16	Kab. Musi Rawas
1606	16	Kab. Musi Banyuasin
1607	16	Kab. Banyu Asin
1608	16	Kab. Ogan Komering Ulu Selatan
1609	16	Kab. Ogan Komering Ulu Timur
1610	16	Kab. Ogan Ilir
1611	16	Kab. Empat Lawang
1612	16	Kab. Penukal Abab Lematang Ilir
1613	16	Kab. Musi Rawas Utara
1671	16	city Palembang
1672	16	city Prabumulih
1673	16	city Pagar Alam
1674	16	city Lubuklinggau
1701	17	Kab. Bengkulu Selatan
1702	17	Kab. Rejang Lebong
1703	17	Kab. Bengkulu Utara
1704	17	Kab. Kaur
1705	17	Kab. Seluma
1706	17	Kab. Mukomuko
1707	17	Kab. Lebong
1708	17	Kab. Kepahiang
1709	17	Kab. Bengkulu Tengah
1771	17	city Bengkulu
1801	18	Kab. Lampung Barat
1802	18	Kab. Tanggamus
1803	18	Kab. Lampung Selatan
1804	18	Kab. Lampung Timur
1805	18	Kab. Lampung Tengah
1806	18	Kab. Lampung Utara
1807	18	Kab. Way Kanan
1808	18	Kab. Tulangbawang
1809	18	Kab. Pesawaran
1810	18	Kab. Pringsewu
1811	18	Kab. Mesuji
1812	18	Kab. Tulang Bawang Barat
1813	18	Kab. Pesisir Barat
1871	18	city Bandar Lampung
1872	18	city Metro
1901	19	Kab. Bangka
1902	19	Kab. Belitung
1903	19	Kab. Bangka Barat
1904	19	Kab. Bangka Tengah
1905	19	Kab. Bangka Selatan
1906	19	Kab. Belitung Timur
1971	19	city Pangkal Pinang
2101	21	Kab. Karimun
2102	21	Kab. Bintan
2103	21	Kab. Natuna
2104	21	Kab. Lingga
2105	21	Kab. Kepulauan Anambas
2171	21	city B A T A M
2172	21	city Tanjung Pinang
3101	31	Kab. Kepulauan Seribu
3171	31	city Jakarta Selatan
3172	31	city Jakarta Timur
3173	31	city Jakarta Pusat
3174	31	city Jakarta Barat
3175	31	city Jakarta Utara
3201	32	Kab. Bogor
3202	32	Kab. Sukabumi
3203	32	Kab. Cianjur
3204	32	Kab. Bandung
3205	32	Kab. Garut
3206	32	Kab. Tasikmalaya
3207	32	Kab. Ciamis
3208	32	Kab. Kuningan
3209	32	Kab. Cirebon
3210	32	Kab. Majalengka
3211	32	Kab. Sumedang
3212	32	Kab. Indramayu
3213	32	Kab. Subang
3214	32	Kab. Purwakarta
3215	32	Kab. Karawang
3216	32	Kab. Bekasi
3217	32	Kab. Bandung Barat
3218	32	Kab. Pangandaran
3271	32	city Bogor
3272	32	city Sukabumi
3273	32	city Bandung
3274	32	city Cirebon
3275	32	city Bekasi
3276	32	city Depok
3277	32	city Cimahi
3278	32	city Tasikmalaya
3279	32	city Banjar
3301	33	Kab. Cilacap
3302	33	Kab. Banyumas
3303	33	Kab. Purbalingga
3304	33	Kab. Banjarnegara
3305	33	Kab. Kebumen
3306	33	Kab. Purworejo
3307	33	Kab. Wonosobo
3308	33	Kab. Magelang
3309	33	Kab. Boyolali
3310	33	Kab. Klaten
3311	33	Kab. Sukoharjo
3312	33	Kab. Wonogiri
3313	33	Kab. Karanganyar
3314	33	Kab. Sragen
3315	33	Kab. Grobogan
3316	33	Kab. Blora
3317	33	Kab. Rembang
3318	33	Kab. Pati
3319	33	Kab. Kudus
3320	33	Kab. Jepara
3321	33	Kab. Demak
3322	33	Kab. Semarang
3323	33	Kab. Temanggung
3324	33	Kab. Kendal
3325	33	Kab. Batang
3326	33	Kab. Pekalongan
3327	33	Kab. Pemalang
3328	33	Kab. Tegal
3329	33	Kab. Brebes
3371	33	city Magelang
3372	33	city Surakarta
3373	33	city Salatiga
3374	33	city Semarang
3375	33	city Pekalongan
3376	33	city Tegal
3401	34	Kab. Kulon Progo
3402	34	Kab. Bantul
3403	34	Kab. Gunung Kidul
3404	34	Kab. Sleman
3471	34	city Yogyakarta
3501	35	Kab. Pacitan
3502	35	Kab. Ponorogo
3503	35	Kab. Trenggalek
3504	35	Kab. Tulungagung
3505	35	Kab. Blitar
3506	35	Kab. Kediri
3507	35	Kab. Malang
3508	35	Kab. Lumajang
3509	35	Kab. Jember
3510	35	Kab. Banyuwangi
3511	35	Kab. Bondowoso
3512	35	Kab. Situbondo
3513	35	Kab. Probolinggo
3514	35	Kab. Pasuruan
3515	35	Kab. Sidoarjo
3516	35	Kab. Mojokerto
3517	35	Kab. Jombang
3518	35	Kab. Nganjuk
3519	35	Kab. Madiun
3520	35	Kab. Magetan
3521	35	Kab. Ngawi
3522	35	Kab. Bojonegoro
3523	35	Kab. Tuban
3524	35	Kab. Lamongan
3525	35	Kab. Gresik
3526	35	Kab. Bangkalan
3527	35	Kab. Sampang
3528	35	Kab. Pamekasan
3529	35	Kab. Sumenep
3571	35	city Kediri
3572	35	city Blitar
3573	35	city Malang
3574	35	city Probolinggo
3575	35	city Pasuruan
3576	35	city Mojokerto
3577	35	city Madiun
3578	35	city Surabaya
3579	35	city Batu
3601	36	Kab. Pandeglang
3602	36	Kab. Lebak
3603	36	Kab. Tangerang
3604	36	Kab. Serang
3671	36	city Tangerang
3672	36	city Cilegon
3673	36	city Serang
3674	36	city Tangerang Selatan
5101	51	Kab. Jembrana
5102	51	Kab. Tabanan
5103	51	Kab. Badung
5104	51	Kab. Gianyar
5105	51	Kab. Klungkung
5106	51	Kab. Bangli
5107	51	Kab. Karang Asem
5108	51	Kab. Buleleng
5171	51	city Denpasar
5201	52	Kab. Lombok Barat
5202	52	Kab. Lombok Tengah
5203	52	Kab. Lombok Timur
5204	52	Kab. Sumbawa
5205	52	Kab. Dompu
5206	52	Kab. Bima
5207	52	Kab. Sumbawa Barat
5208	52	Kab. Lombok Utara
5271	52	city Mataram
5272	52	city Bima
5301	53	Kab. Sumba Barat
5302	53	Kab. Sumba Timur
5303	53	Kab. Kupang
5304	53	Kab. Timor Tengah Selatan
5305	53	Kab. Timor Tengah Utara
5306	53	Kab. Belu
5307	53	Kab. Alor
5308	53	Kab. Lembata
5309	53	Kab. Flores Timur
5310	53	Kab. Sikka
5311	53	Kab. Ende
5312	53	Kab. Ngada
5313	53	Kab. Manggarai
5314	53	Kab. Rote Ndao
5315	53	Kab. Manggarai Barat
5316	53	Kab. Sumba Tengah
5317	53	Kab. Sumba Barat Daya
5318	53	Kab. Nagekeo
5319	53	Kab. Manggarai Timur
5320	53	Kab. Sabu Raijua
5321	53	Kab. Malaka
5371	53	city Kupang
6101	61	Kab. Sambas
6102	61	Kab. Bengkayang
6103	61	Kab. Landak
6104	61	Kab. Mempawah
6105	61	Kab. Sanggau
6106	61	Kab. Ketapang
6107	61	Kab. Sintang
6108	61	Kab. Kapuas Hulu
6109	61	Kab. Sekadau
6110	61	Kab. Melawi
6111	61	Kab. Kayong Utara
6112	61	Kab. Kubu Raya
6171	61	city Pontianak
6172	61	city Singkawang
6201	62	Kab. citywaringin Barat
6202	62	Kab. citywaringin Timur
6203	62	Kab. Kapuas
6204	62	Kab. Barito Selatan
6205	62	Kab. Barito Utara
6206	62	Kab. Sukamara
6207	62	Kab. Lamandau
6208	62	Kab. Seruyan
6209	62	Kab. Katingan
6210	62	Kab. Pulang Pisau
6211	62	Kab. Gunung Mas
6212	62	Kab. Barito Timur
6213	62	Kab. Murung Raya
6271	62	city Palangka Raya
6301	63	Kab. Tanah Laut
6302	63	Kab. city Baru
6303	63	Kab. Banjar
6304	63	Kab. Barito Kuala
6305	63	Kab. Tapin
6306	63	Kab. Hulu Sungai Selatan
6307	63	Kab. Hulu Sungai Tengah
6308	63	Kab. Hulu Sungai Utara
6309	63	Kab. Tabalong
6310	63	Kab. Tanah Bumbu
6311	63	Kab. Balangan
6371	63	city Banjarmasin
6372	63	city Banjar Baru
6401	64	Kab. Paser
6402	64	Kab. Kutai Barat
6403	64	Kab. Kutai Kartanegara
6404	64	Kab. Kutai Timur
6405	64	Kab. Berau
6409	64	Kab. Penatime Paser Utara
6411	64	Kab. Mahakam Hulu
6471	64	city Balikpapan
6472	64	city Samarinda
6474	64	city Bontang
6501	65	Kab. Malinau
6502	65	Kab. Bulungan
6503	65	Kab. Tana Tidung
6504	65	Kab. Nunukan
6571	65	city Tarakan
7101	71	Kab. Bolaang Mongondow
7102	71	Kab. Minahasa
7103	71	Kab. Kepulauan Sangihe
7104	71	Kab. Kepulauan Talaud
7105	71	Kab. Minahasa Selatan
7106	71	Kab. Minahasa Utara
7107	71	Kab. Bolaang Mongondow Utara
7108	71	Kab. Siau Tagulandang Biaro
7109	71	Kab. Minahasa Tenggara
7110	71	Kab. Bolaang Mongondow Selatan
7111	71	Kab. Bolaang Mongondow Timur
7171	71	city Manado
7172	71	city Bitung
7173	71	city Tomohon
7174	71	city citymobagu
7201	72	Kab. Banggai Kepulauan
7202	72	Kab. Banggai
7203	72	Kab. Morowali
7204	72	Kab. Poso
7205	72	Kab. Donggala
7206	72	Kab. Toli-Toli
7207	72	Kab. Buol
7208	72	Kab. Parigi Moutong
7209	72	Kab. Tojo Una-Una
7210	72	Kab. Sigi
7211	72	Kab. Banggai Laut
7212	72	Kab. Morowali Utara
7271	72	city Palu
7301	73	Kab. Kepulauan Selayar
7302	73	Kab. Bulukumba
7303	73	Kab. Bantaeng
7304	73	Kab. Jeneponto
7305	73	Kab. Takalar
7306	73	Kab. Gowa
7307	73	Kab. Sinjai
7308	73	Kab. Maros
7309	73	Kab. Pangkajene Dan Kepulauan
7310	73	Kab. Barru
7311	73	Kab. Bone
7312	73	Kab. Soppeng
7313	73	Kab. Wajo
7314	73	Kab. Sidenreng Rappang
7315	73	Kab. Pinrang
7316	73	Kab. Enrekang
7317	73	Kab. Luwu
7318	73	Kab. Tana Toraja
7322	73	Kab. Luwu Utara
7325	73	Kab. Luwu Timur
7326	73	Kab. Toraja Utara
7371	73	city Makassar
7372	73	city Parepare
7373	73	city Palopo
7401	74	Kab. Buton
7402	74	Kab. Muna
7403	74	Kab. Konawe
7404	74	Kab. Kolaka
7405	74	Kab. Konawe Selatan
7406	74	Kab. Bombana
7407	74	Kab. Wakatobi
7408	74	Kab. Kolaka Utara
7409	74	Kab. Buton Utara
7410	74	Kab. Konawe Utara
7411	74	Kab. Kolaka Timur
7412	74	Kab. Konawe Kepulauan
7413	74	Kab. Muna Barat
7414	74	Kab. Buton Tengah
7415	74	Kab. Buton Selatan
7471	74	city Kendari
7472	74	city Baubau
7501	75	Kab. Boalemo
7502	75	Kab. Gorontalo
7503	75	Kab. Pohuwato
7504	75	Kab. Bone Bolango
7505	75	Kab. Gorontalo Utara
7571	75	city Gorontalo
7601	76	Kab. Majene
7602	76	Kab. Polewali Mandar
7603	76	Kab. Mamasa
7604	76	Kab. Mamuju
7605	76	Kab. Mamuju Utara
7606	76	Kab. Mamuju Tengah
8101	81	Kab. Maluku Tenggara Barat
8102	81	Kab. Maluku Tenggara
8103	81	Kab. Maluku Tengah
8104	81	Kab. Buru
8105	81	Kab. Kepulauan Aru
8106	81	Kab. Seram Bagian Barat
8107	81	Kab. Seram Bagian Timur
8108	81	Kab. Maluku Barat Daya
8109	81	Kab. Buru Selatan
8171	81	city Ambon
8172	81	city Tual
8201	82	Kab. Halmahera Barat
8202	82	Kab. Halmahera Tengah
8203	82	Kab. Kepulauan Sula
8204	82	Kab. Halmahera Selatan
8205	82	Kab. Halmahera Utara
8206	82	Kab. Halmahera Timur
8207	82	Kab. Pulau Morotai
8208	82	Kab. Pulau Taliabu
8271	82	city Ternate
8272	82	city Tidore Kepulauan
9101	91	Kab. Fakfak
9102	91	Kab. Kaimana
9103	91	Kab. Teluk Wondama
9104	91	Kab. Teluk Bintuni
9105	91	Kab. Manokwari
9106	91	Kab. Sorong Selatan
9107	91	Kab. Sorong
9108	91	Kab. Raja Ampat
9109	91	Kab. Tambrauw
9110	91	Kab. Maybrat
9111	91	Kab. Manokwari Selatan
9112	91	Kab. Pegunungan Arfak
9171	91	city Sorong
9401	94	Kab. Merauke
9402	94	Kab. Jayawijaya
9403	94	Kab. Jayapura
9404	94	Kab. Nabire
9408	94	Kab. Kepulauan Yapen
9409	94	Kab. Biak Numfor
9410	94	Kab. Paniai
9411	94	Kab. Puncak Jaya
9412	94	Kab. Mimika
9413	94	Kab. Boven Digoel
9414	94	Kab. Mappi
9415	94	Kab. Asmat
9416	94	Kab. Yahukimo
9417	94	Kab. Pegunungan Bintang
9418	94	Kab. Tolikara
9419	94	Kab. Sarmi
9420	94	Kab. Keerom
9426	94	Kab. Waropen
9427	94	Kab. Supiori
9428	94	Kab. Mamberamo Raya
9429	94	Kab. Nduga
9430	94	Kab. Lanny Jaya
9431	94	Kab. Mamberamo Tengah
9432	94	Kab. Yalimo
9433	94	Kab. Puncak
9434	94	Kab. Dogiyai
9435	94	Kab. Intan Jaya
9436	94	Kab. Deiyai
9471	94	city Jayapura
\.


--
-- PostgreSQL database dump complete
--


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
-- Data for Name: m_subdistrict; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY m_subdistrict (subdistrict_id, regency_id, name_subdistrict) FROM stdin;
1101010	1101	Teupah Selatan
1101020	1101	Simeulue Timur
1101021	1101	Teupah Barat
1101022	1101	Teupah Tengah
1101030	1101	Simeulue Tengah
1101031	1101	Teluk Dalam
1101032	1101	Simeulue Cut
1101040	1101	Salang
1101050	1101	Simeulue Barat
1101051	1101	Alafan
1102010	1102	Pulau Banyak
1102011	1102	Pulau Banyak Barat
1102020	1102	Singkil
1102021	1102	Singkil Utara
1102022	1102	Kuala Baru
1102030	1102	Simpang Kanan
1102031	1102	Gunung Meriah
1102032	1102	Danau Paris
1102033	1102	Suro
1102042	1102	Singkohor
1102043	1102	city Baharu
1103010	1103	Trumon
1103011	1103	Trumon Timur
1103012	1103	Trumon Tengah
1103020	1103	Bakongan
1103021	1103	Bakongan Timur
1103022	1103	city Bahagia
1103030	1103	Kluet Selatan
1103031	1103	Kluet Timur
1103040	1103	Kluet Utara
1103041	1103	Pasie Raja
1103042	1103	Kluet Tengah
1103050	1103	Tapak Tuan
1103060	1103	Sama Dua
1103070	1103	Sawang
1103080	1103	Meukek
1103090	1103	Labuhan Haji
1103091	1103	Labuhan Haji Timur
1103092	1103	Labuhan Haji Barat
1104010	1104	Lawe Alas
1104011	1104	Babul Rahmah
1104012	1104	Tanoh Alas
1104020	1104	Lawe Sigala-Gala
1104021	1104	Babul Makmur
1104022	1104	Semadam
1104023	1104	Leuser
1104030	1104	Bambel
1104031	1104	Bukit Tusam
1104032	1104	Lawe Sumur
1104040	1104	Babussalam
1104041	1104	Lawe month
1104050	1104	Badar
1104051	1104	Darul Hasanah
1104052	1104	Ketambe
1104053	1104	Deleng Pokhkisen
1105080	1105	Serba Jadi
1105081	1105	Simpang Jernih
1105082	1105	Peunaron
1105090	1105	Birem Bayeun
1105100	1105	Rantau Selamat
1105101	1105	Sungai Raya
1105110	1105	Peureulak
1105111	1105	Peureulak Timur
1105112	1105	Peureulak Barat
1105120	1105	Ranto Peureulak
1105130	1105	Idi Rayeuk
1105131	1105	Peudawa
1105132	1105	Banda Alam
1105133	1105	Idi Tunong
1105134	1105	Darul Ihsan
1105135	1105	Idi Timur
1105140	1105	Darul Aman
1105150	1105	Nurussalam
1105151	1105	Darul Falah
1105160	1105	Julok
1105161	1105	Indra Makmur
1105170	1105	Pante Bidari
1105180	1105	Simpang Ulim
1105181	1105	Madat
1106010	1106	Linge
1106011	1106	Atu Lintang
1106012	1106	Jagong Jeget
1106020	1106	Bintang
1106031	1106	Lut Tawar
1106032	1106	Kebayakan
1106040	1106	Pegasing
1106041	1106	Bies
1106050	1106	Bebesen
1106051	1106	Kute Panang
1106060	1106	Silih Nara
1106061	1106	Ketol
1106062	1106	Celala
1106063	1106	Rusip Antara
1107050	1107	Johan Pahlawan
1107060	1107	Samatiga
1107061	1107	Bubon
1107062	1107	Arongan Lambalek
1107070	1107	Woyla
1107071	1107	Woyla Barat
1107072	1107	Woyla Timur
1107080	1107	Kaway Xvi
1107081	1107	Meureubo
1107082	1107	Pantai Ceuremen
1107083	1107	Panton Reu
1107090	1107	Sungai Mas
1108010	1108	Lhoong
1108020	1108	Lhoknga
1108021	1108	Leupung
1108030	1108	Indrapuri
1108031	1108	Kuta Cot Glie
1108040	1108	Seulimeum
1108041	1108	city Jantho
1108042	1108	Lembah Seulawah
1108050	1108	Mesjid Raya
1108060	1108	Darussalam
1108061	1108	Baitussalam
1108070	1108	Kuta Baro
1108080	1108	Montasik
1108081	1108	Blang Bintang
1108090	1108	Ingin Jaya
1108091	1108	Krueng Barona Jaya
1108100	1108	Suka Makmur
1108101	1108	Kuta Malaka
1108102	1108	Simpang Tiga
1108110	1108	Darul Imarah
1108111	1108	Darul Kamal
1108120	1108	Peukan Bada
1108130	1108	Pulo Aceh
1109010	1109	Geumpang
1109011	1109	Mane
1109070	1109	Glumpang Tiga
1109071	1109	Glumpang Baro
1109080	1109	Mutiara
1109081	1109	Mutiara Timur
1109090	1109	Tiro/Truseb
1109100	1109	Tangse
1109111	1109	Keumala
1109112	1109	Titeue
1109120	1109	Sakti
1109130	1109	Mila
1109140	1109	Padang Tiji
1109150	1109	Delima
1109151	1109	Grong Grong
1109160	1109	Indrajaya
1109170	1109	Peukan Baro
1109180	1109	Kembang Tanjong
1109190	1109	Simpang Tiga
1109200	1109	city Sigli
1109210	1109	Pidie
1109220	1109	Batee
1109230	1109	Muara Tiga
1110010	1110	Samalanga
1110011	1110	Simpang Mamplam
1110020	1110	Pandrah
1110030	1110	Jeunieb
1110031	1110	Peulimbang
1110040	1110	Peudada
1110050	1110	Juli
1110060	1110	Jeumpa
1110061	1110	city Juang
1110062	1110	Kuala
1110070	1110	Jangka
1110080	1110	Peusangan
1110081	1110	Peusangan Selatan
1110082	1110	Peusangan Siblah Krueng
1110090	1110	Makmur
1110100	1110	Ganda Pura
1110101	1110	Kuta Blang
1111010	1111	Sawang
1111020	1111	Nisam
1111021	1111	Nisam Antara
1111022	1111	Banda Baro
1111030	1111	Kuta Makmur
1111031	1111	Simpang Keramat
1111040	1111	Syamtalira Bayu
1111041	1111	Geureudong Pase
1111050	1111	Meurah Mulia
1111060	1111	Matangkuli
1111061	1111	Paya Bakong
1111062	1111	Pirak Timu
1111070	1111	Cot Girek
1111080	1111	Tanah timebo Aye
1111081	1111	Langkahan
1111090	1111	Seunuddon
1111100	1111	Baktiya
1111101	1111	Baktiya Barat
1111110	1111	Lhoksukon
1111120	1111	Tanah Luas
1111121	1111	Nibong
1111130	1111	Samudera
1111140	1111	Syamtalira Aron
1111150	1111	Tanah Pasir
1111151	1111	Lapang
1111160	1111	Muara Batu
1111170	1111	Dewantara
1112010	1112	Manggeng
1112011	1112	Lembah Sabil
1112020	1112	Tangan-Tangan
1112021	1112	Setia
1112030	1112	Blang Pidie
1112031	1112	Jeumpa
1112040	1112	Susoh
1112050	1112	Kuala Batee
1112060	1112	Babah Rot
1113010	1113	Kuta Panjang
1113011	1113	Blang Jerango
1113020	1113	Blangkejeren
1113021	1113	Putri Betung
1113022	1113	Dabun Gelang
1113023	1113	Blang Pegayon
1113030	1113	Pining
1113040	1113	Rikit Gaib
1113041	1113	Pantan Cuaca
1113050	1113	Terangun
1113051	1113	Tripe Jaya
1114010	1114	Tamiang Hulu
1114011	1114	Bandar Pusaka
1114020	1114	Kejuruan Muda
1114021	1114	Tenggulun
1114030	1114	Rantau
1114040	1114	city Kuala Simpang
1114050	1114	Seruway
1114060	1114	Bendahara
1114061	1114	Banda Mulia
1114070	1114	Karang Baru
1114071	1114	Sekerak
1114080	1114	Manyak Payed
1115010	1115	Darul Makmur
1115011	1115	Tripa Makmur
1115020	1115	Kuala
1115021	1115	Kuala Pesisir
1115022	1115	Tadu Raya
1115030	1115	Beutong
1115031	1115	Beutong Ateuh Banggalang
1115040	1115	Seunagan
1115041	1115	Suka Makmue
1115050	1115	Seunagan Timur
1116010	1116	Teunom
1116011	1116	Pasie Raya
1116020	1116	Panga
1116030	1116	Krueng Sabee
1116040	1116	Setia Bakti
1116050	1116	Sampoiniet
1116051	1116	Darul Hikmah
1116060	1116	Jaya
1116061	1116	Indra Jaya
1117010	1117	Timang Gajah
1117011	1117	Gajah Putih
1117020	1117	Pintu Rime Gayo
1117030	1117	Bukit
1117040	1117	Wih Pesam
1117050	1117	Bandar
1117051	1117	Bener Kelipah
1117060	1117	Syiah Utama
1117061	1117	Mesidah
1117070	1117	Permata
1118010	1118	Meureudu
1118020	1118	Meurah Dua
1118030	1118	Bandar Dua
1118040	1118	Jangka Buya
1118050	1118	Ulim
1118060	1118	Trienggadeng
1118070	1118	Panteraja
1118080	1118	Bandar Baru
1171010	1171	Meuraxa
1171011	1171	Jaya Baru
1171012	1171	Banda Raya
1171020	1171	Baiturrahman
1171021	1171	Lueng Bata
1171030	1171	Kuta Alam
1171031	1171	Kuta Raja
1171040	1171	Syiah Kuala
1171041	1171	Ulee Kareng
1172010	1172	Sukajaya
1172020	1172	Sukakarya
1173010	1173	Langsa Timur
1173011	1173	Langsa Lama
1173020	1173	Langsa Barat
1173021	1173	Langsa Baro
1173030	1173	Langsa city
1174010	1174	Blang Mangat
1174020	1174	Muara Dua
1174021	1174	Muara Satu
1174030	1174	Banda Sakti
1175010	1175	Simpang Kiri
1175020	1175	Penanggalan
1175030	1175	Rundeng
1175040	1175	Sultan Daulat
1175050	1175	Longkib
1201060	1201	Idano Gawo
1201061	1201	Bawolato
1201062	1201	Ulugawo
1201070	1201	Gido
1201071	1201	Sogaeadu
1201081	1201	Ma U
1201082	1201	Somolo - Molo
1201130	1201	Hiliduho
1201131	1201	Hili Serangkai
1201132	1201	Botomuzoi
1202010	1202	Batahan
1202011	1202	Sinunukan
1202020	1202	Batang Natal
1202021	1202	Lingga Bayu
1202022	1202	Ranto Baek
1202030	1202	citynopan
1202031	1202	Ulu Pungkut
1202032	1202	Tambangan
1202033	1202	Lembah Sorik Marapi
1202034	1202	Puncak Sorik Marapi
1202040	1202	Muara Sipongi
1202041	1202	Pakantan
1202050	1202	Panyabungan
1202051	1202	Panyabungan Selatan
1202052	1202	Panyabungan Barat
1202053	1202	Panyabungan Utara
1202054	1202	Panyabungan Timur
1202055	1202	Huta Bargot
1202060	1202	Natal
1202070	1202	Muara Batang Gadis
1202080	1202	Siabu
1202081	1202	Bukit Malintang
1202082	1202	Naga Juang
1203010	1203	Batang Angkola
1203011	1203	Sayur Matinggi
1203012	1203	Tano Tombangan Angkola
1203070	1203	Angkola Timur
1203080	1203	Angkola Selatan
1203090	1203	Angkola  Barat
1203091	1203	Angkola Sangkunur
1203100	1203	Batang Toru
1203101	1203	Marancar
1203102	1203	Muara Batang Toru
1203110	1203	Sipirok
1203120	1203	Arse
1203160	1203	Saipar Dolok Hole
1203161	1203	Aek Bilah
1204010	1204	Pinang Sori
1204011	1204	Badiri
1204020	1204	Sibabangun
1204021	1204	Lumut
1204022	1204	Sukabangun
1204030	1204	Pandan
1204031	1204	Tukka
1204032	1204	Sarudik
1204040	1204	Tapian Nauli
1204041	1204	Sitahuis
1204050	1204	Kolang
1204060	1204	Sorkam
1204061	1204	Sorkam Barat
1204062	1204	Pasaribu Tobing
1204070	1204	Barus
1204071	1204	Sosor Gadong
1204072	1204	Andam Dewi
1204073	1204	Barus Utara
1204080	1204	Manduamas
1204081	1204	Sirandorung
1205030	1205	Parmonangan
1205040	1205	Adiankoting
1205050	1205	Sipoholon
1205060	1205	Tarutung
1205061	1205	Siatas Barita
1205070	1205	Pahae Julu
1205080	1205	Pahae Jae
1205081	1205	Purbatua
1205082	1205	Simangumban
1205090	1205	Pangaribuan
1205100	1205	Garoga
1205110	1205	Sipahutar
1205120	1205	Siborongborong
1205130	1205	Pagaran
1205180	1205	Muara
1206030	1206	Balige
1206031	1206	Tampahan
1206040	1206	Laguboti
1206050	1206	Habinsaran
1206051	1206	Borbor
1206052	1206	Nassau
1206060	1206	Silaen
1206061	1206	Sigumpar
1206070	1206	Porsea
1206071	1206	Pintu Pohan Meranti
1206072	1206	Siantar Narumonda
1206073	1206	Parmaksian
1206080	1206	Lumban Julu
1206081	1206	Uluan
1206082	1206	Ajibata
1206083	1206	Bonatua Lunasi
1207050	1207	Bilah Hulu
1207070	1207	Pangkatan
1207080	1207	Bilah Barat
1207130	1207	Bilah Hilir
1207140	1207	Panai Hulu
1207150	1207	Panai Tengah
1207160	1207	Panai Hilir
1207210	1207	Rantau Selatan
1207220	1207	Rantau Utara
1208010	1208	Bandar Pasir Mandoge
1208020	1208	Bandar Pulau
1208021	1208	Aek Songsongan
1208022	1208	Rahuning
1208030	1208	Pulau Rakyat
1208031	1208	Aek Kuasan
1208032	1208	Aek Ledong
1208040	1208	Sei Kepayang
1208041	1208	Sei Kepayang Barat
1208042	1208	Sei Kepayang Timur
1208050	1208	Tanjung Balai
1208060	1208	Simpang Empat
1208061	1208	Teluk Dalam
1208070	1208	Air Batu
1208071	1208	Sei Dadap
1208080	1208	Buntu Pane
1208081	1208	Tinggi Raja
1208082	1208	Setia Janji
1208090	1208	Meranti
1208091	1208	Pulo Bandring
1208092	1208	Rawang Panca Arga
1208100	1208	Air Joman
1208101	1208	Silau Laut
1208160	1208	Kisaran Barat
1208170	1208	Kisaran Timur
1209010	1209	Silimakuta
1209011	1209	Pematang Silimahuta
1209020	1209	Purba
1209021	1209	Haranggaol Horison
1209040	1209	Sidamanik
1209041	1209	Pematang Sidamanik
1209050	1209	Girsang Sipangan Bolon
1209060	1209	Tanah Jawa
1209061	1209	Hatonduhan
1209070	1209	Dolok Panribuan
1209080	1209	Jorlang Hataran
1209090	1209	Panei
1209091	1209	Panombean Panei
1209100	1209	Raya
1209101	1209	Dolog Masagal
1209110	1209	Dolok Silau
1209120	1209	Silau Kahean
1209130	1209	Raya Kahean
1209140	1209	Tapian Dolok
1209150	1209	Dolok Batu Nanggar
1209160	1209	Siantar
1209161	1209	Gunung Malela
1209162	1209	Gunung Maligas
1209170	1209	Hutabayu Raja
1209171	1209	Jawa Maraja Bah timebi
1209180	1209	Pematang Bandar
1209181	1209	Bandar Huluan
1209190	1209	Bandar
1209191	1209	Bandar Masilam
1209200	1209	Bosar Maligas
1209210	1209	Ujung Padang
1210030	1210	Sidikalang
1210031	1210	Berampu
1210032	1210	Sitinjo
1210040	1210	Parbuluan
1210050	1210	Sumbul
1210051	1210	Silahisabungan
1210060	1210	Silima Pungga-Pungga
1210061	1210	Lae Parira
1210070	1210	Siempat Nempu
1210080	1210	Siempat Nempu Hulu
1210090	1210	Siempat Nempu Hilir
1210100	1210	Tiga Lingga
1210101	1210	Gunung Sitember
1210110	1210	Pegagan Hilir
1210120	1210	Tanah Pinem
1211010	1211	Mardingding
1211020	1211	Laubaleng
1211030	1211	Tiga Binanga
1211040	1211	Juhar
1211050	1211	Munte
1211060	1211	Kuta Buluh
1211070	1211	Payung
1211071	1211	Tiganderket
1211080	1211	Simpang Empat
1211081	1211	namen Teran
1211082	1211	Merdeka
1211090	1211	Kabanjahe
1211100	1211	Berastagi
1211110	1211	Tigapanah
1211111	1211	Dolat Rayat
1211120	1211	Merek
1211130	1211	Barusjahe
1212010	1212	Gunung Meriah
1212020	1212	Sinembah Tanjung Muda Hulu
1212030	1212	Sibolangit
1212040	1212	Kutalimbaru
1212050	1212	Pancur Batu
1212060	1212	Namo Rambe
1212070	1212	Biru-Biru
1212080	1212	Sinembah Tanjung Muda Hilir
1212090	1212	Bangun Purba
1212190	1212	Galang
1212200	1212	Tanjung Morawa
1212210	1212	Patumbak
1212220	1212	Deli Tua
1212230	1212	Sunggal
1212240	1212	Hamparan Perak
1212250	1212	Labuhan Deli
1212260	1212	Percut Sei Tuan
1212270	1212	Batang Kuis
1212280	1212	Pantai Labu
1212290	1212	Beringin
1212300	1212	Lubuk Pakam
1212310	1212	Pagar Merbau
1213010	1213	Bohorok
1213011	1213	Sirapit
1213020	1213	Salapian
1213021	1213	Kutambaru
1213030	1213	Sei Bingai
1213040	1213	Kuala
1213050	1213	Selesai
1213060	1213	Binjai
1213070	1213	Stabat
1213080	1213	Wampu
1213090	1213	Batang Serangan
1213100	1213	Sawit Seberang
1213110	1213	Padang Tualang
1213120	1213	Hinai
1213130	1213	Secanggang
1213140	1213	Tanjung Pura
1213150	1213	Gebang
1213160	1213	Babalan
1213170	1213	Sei Lepan
1213180	1213	Brandan Barat
1213190	1213	Besitang
1213200	1213	Pangkalan Susu
1213201	1213	Pematang Jaya
1214010	1214	Hibala
1214011	1214	Tanah Masa
1214020	1214	Pulau-Pulau Batu
1214021	1214	Pulau-Pulau Batu Timur
1214022	1214	Simuk
1214023	1214	Pulau-Pulau Batu Barat
1214024	1214	Pulau-Pulau Batu Utara
1214030	1214	Teluk Dalam
1214031	1214	Fanayama
1214032	1214	Toma
1214033	1214	Maniamolo
1214034	1214	Mazino
1214035	1214	Luahagundre Maniamolo
1214036	1214	Onolalu
1214040	1214	Amandraya
1214041	1214	Aramo
1214042	1214	Ulususua
1214050	1214	Lahusa
1214051	1214	Siduaori
1214052	1214	Somambawa
1214060	1214	Gomo
1214061	1214	Susua
1214062	1214	Mazo
1214063	1214	Umbunasi
1214064	1214	Idanotae
1214065	1214	Uluidanotae
1214066	1214	Boronadu
1214070	1214	Lolomatua
1214071	1214	Ulunoyo
1214072	1214	Huruna
1214080	1214	Lolowa'u
1214081	1214	Hilimegai
1214082	1214	Oou
1214083	1214	Onohazumba
1214084	1214	Hilisalawaahe
1215010	1215	Pakkat
1215020	1215	Onan Ganjang
1215030	1215	Sitimea Polang
1215040	1215	Dolok Sanggul
1215050	1215	Lintong Nihuta
1215060	1215	Paranginan
1215070	1215	Bakti Raja
1215080	1215	Pollung
1215090	1215	Parlilitan
1215100	1215	Tara Bintang
1216010	1216	Salak
1216011	1216	Sitellu Tali Urang Jehe
1216012	1216	Pagindar
1216013	1216	Sitellu Tali Urang Julu
1216014	1216	Pergetteng-Getteng Sengkut
1216020	1216	Kerajaan
1216021	1216	Tinada
1216022	1216	Siempat Rube
1217010	1217	Sianjur Mula Mula
1217020	1217	Harian
1217030	1217	Sitio-Tio
1217040	1217	Onan Runggu
1217050	1217	Nainggolan
1217060	1217	Palipi
1217070	1217	Ronggur Nihuta
1217080	1217	Pangururan
1217090	1217	Simanindo
1218010	1218	cityrih
1218011	1218	Silinda
1218012	1218	Bintang Bayu
1218020	1218	Dolok Masihul
1218021	1218	Serbajadi
1218030	1218	Sipispis
1218040	1218	Dolok Merawan
1218050	1218	Tebingtinggi
1218051	1218	Tebing Syahbandar
1218060	1218	Bandar Khalipah
1218070	1218	Tanjung Beringin
1218080	1218	Sei Rampah
1218081	1218	Sei Bamban
1218090	1218	Teluk Mengkudu
1218100	1218	Perbaungan
1218101	1218	Pegajahan
1218110	1218	Pantai Cermin
1219010	1219	Sei Balai
1219020	1219	Tanjung Tiram
1219021	1219	Nibung Hangus
1219030	1219	Talawi
1219031	1219	Datuk Tanah Datar
1219040	1219	Limapuluh
1219041	1219	Lima Puluh Pesisir
1219042	1219	Datuk Lima Puluh
1219050	1219	Air Putih
1219060	1219	Sei Suka
1219061	1219	Laut Tador
1219070	1219	Medang Deras
1220010	1220	Batang Onang
1220020	1220	Padang Bolak Julu
1220030	1220	Portibi
1220040	1220	Padang Bolak
1220041	1220	Padang Bolak Tenggara
1220050	1220	Simangambat
1220051	1220	Ujung Batu
1220060	1220	Halongonan
1220061	1220	Halongonan Timur
1220070	1220	Dolok
1220080	1220	Dolok Sigompulon
1220090	1220	Hulu Sihapas
1221010	1221	Sosopan
1221020	1221	Ulu Barumun
1221030	1221	Barumun
1221031	1221	Barumun Selatan
1221040	1221	Lubuk Barumun
1221050	1221	Sosa
1221060	1221	Batang Lubu Sutam
1221070	1221	Huta Raja Tinggi
1221080	1221	Huristak
1221090	1221	Barumun Tengah
1221091	1221	Aek Nabara Barumun
1221092	1221	Sihapas Barumun
1222010	1222	Sungai Kanan
1222020	1222	Torgamba
1222030	1222	city Pinang
1222040	1222	Silangkitang
1222050	1222	Kampung Rakyat
1223010	1223	Na Ix-X
1223020	1223	Marbau
1223030	1223	Aek Kuo
1223040	1223	Aek Natas
1223050	1223	Kualuh Selatan
1223060	1223	Kualuh Hilir
1223070	1223	Kualuh Hulu
1223080	1223	Kualuh Leidong
1224010	1224	Tugala Oyo
1224020	1224	Alasa
1224030	1224	Alasa Talu Muzoi
1224040	1224	Namohalu Esiwa
1224050	1224	Sitolu Ori
1224060	1224	Tuhemberua
1224070	1224	Sawo
1224080	1224	Lotu
1224090	1224	Lahewa Timur
1224100	1224	Afulu
1224110	1224	Lahewa
1225010	1225	Sirombu
1225020	1225	Lahomi
1225030	1225	Ulu Moro O
1225040	1225	Lolofitu Moi
1225050	1225	Mandrehe Utara
1225060	1225	Mandrehe
1225070	1225	Mandrehe Barat
1225080	1225	Moro O
1271010	1271	Sibolga Utara
1271020	1271	Sibolga city
1271030	1271	Sibolga Selatan
1271031	1271	Sibolga Sambas
1272010	1272	Datuk Bandar
1272011	1272	Datuk Bandar Timur
1272020	1272	Tanjung Balai Selatan
1272030	1272	Tanjung Balai Utara
1272040	1272	Sei Tualang Raso
1272050	1272	Teluk Nibung
1273010	1273	Siantar Marihat
1273011	1273	Siantar Marimbun
1273020	1273	Siantar Selatan
1273030	1273	Siantar Barat
1273040	1273	Siantar Utara
1273050	1273	Siantar Timur
1273060	1273	Siantar Martoba
1273061	1273	Siantar Sitalasari
1274010	1274	Padang Hulu
1274011	1274	Tebing Tinggi city
1274020	1274	Rambutan
1274021	1274	Bajenis
1274030	1274	Padang Hilir
1275010	1275	Medan Tuntungan
1275020	1275	Medan Johor
1275030	1275	Medan Amplas
1275040	1275	Medan Denai
1275050	1275	Medan Area
1275060	1275	Medan city
1275070	1275	Medan Maimun
1275080	1275	Medan Polonia
1275090	1275	Medan Baru
1275100	1275	Medan Selayang
1275110	1275	Medan Sunggal
1275120	1275	Medan Helvetia
1275130	1275	Medan Petisah
1275140	1275	Medan Barat
1275150	1275	Medan Timur
1275160	1275	Medan Perjuangan
1275170	1275	Medan Tembung
1275180	1275	Medan Deli
1275190	1275	Medan Labuhan
1275200	1275	Medan Marelan
1275210	1275	Medan Belawan
1276010	1276	Binjai Selatan
1276020	1276	Binjai city
1276030	1276	Binjai Timur
1276040	1276	Binjai Utara
1276050	1276	Binjai Barat
1277010	1277	Padangsidimpuan Tenggara
1277020	1277	Padangsidimpuan Selatan
1277030	1277	Padangsidimpuan Batunadua
1277040	1277	Padangsidimpuan Utara
1277050	1277	Padangsidimpuan Hutaimbaru
1277051	1277	Padangsidimpuan Angkola Julu
1278010	1278	Gunungsitoli Idanoi
1278020	1278	Gunungsitoli Selatan
1278030	1278	Gunungsitoli Barat
1278040	1278	Gunung Sitoli
1278050	1278	Gunungsitoli Alo Oa
1278060	1278	Gunungsitoli Utara
1301011	1301	Pagai Selatan
1301012	1301	Sikakap
1301013	1301	Pagai Utara
1301021	1301	Sipora Selatan
1301022	1301	Sipora Utara
1301030	1301	Siberut Selatan
1301031	1301	Seberut Barat Daya
1301032	1301	Siberut Tengah
1301040	1301	Siberut Utara
1301041	1301	Siberut Barat
1302011	1302	Silaut
1302012	1302	Lunang
1302020	1302	Basa Ampek Balai Tapan
1302021	1302	Ranah Ampek Hulu Tapan
1302030	1302	Pancung Soal
1302031	1302	Airpura
1302040	1302	Linggo Sari Baganti
1302050	1302	Ranah Pesisir
1302060	1302	Lengayang
1302070	1302	Sutera
1302080	1302	Batang Kapas
1302090	1302	Iv Jurai
1302100	1302	Bayang
1302101	1302	Iv  Nagari Bayang Utara
1302110	1302	Koto Xi Tarusan
1303040	1303	Pantai Cermin
1303050	1303	Lembah Gumanti
1303051	1303	Hiliran Gumanti
1303060	1303	Payung Sekaki
1303061	1303	Tigo Lurah
1303070	1303	Lembang Jaya
1303071	1303	Danau Kembar
1303080	1303	Gunung Talang
1303090	1303	Bukit Sundi
1303100	1303	Ix Koto Sungai Lasi
1303110	1303	Kubung
1303120	1303	X Koto Diatas
1303130	1303	X Koto Singkarak
1303140	1303	Junjung Sirih
1304050	1304	Kamang Baru
1304060	1304	Tanjung Gadang
1304070	1304	Sijunjung
1304071	1304	Lubuk Tarok
1304080	1304	Iv Nagari
1304090	1304	Kupitan
1304100	1304	Koto Tujuh
1304110	1304	Sumpur Kudus
1305010	1305	Sepuluh Koto
1305020	1305	Batipuh
1305021	1305	Batipuh Selatan
1305030	1305	Pariangan
1305040	1305	Rambatan
1305050	1305	Lima Kaum
1305060	1305	Tanjung Emas
1305070	1305	Padang Ganting
1305080	1305	Lintau Buo
1305081	1305	Lintau Buo Utara
1305090	1305	Sungayang
1305100	1305	Sungai Tarab
1305110	1305	Salimpaung
1305111	1305	Tanjung Baru
1306010	1306	Batang Anai
1306020	1306	Lubuk Alung
1306021	1306	Sintuk Toboh Gadang
1306030	1306	Ulakan Tapakis
1306040	1306	Nan Sabaris
1306050	1306	2 X 11 Enam Lingkung
1306051	1306	Enam Lingkung
1306052	1306	2 X 11 Kayu Tanam
1306060	1306	Vii Koto Sungai Sariak
1306061	1306	Patamuan
1306062	1306	Padang Sago
1306070	1306	V Koto Kp Dalam
1306071	1306	V Koto Timur
1306080	1306	Sungai Limau
1306081	1306	Batang Gasan
1306090	1306	Sungai Geringging
1306100	1306	Iv Koto Aur Malintang
1307010	1307	Tanjung Mutiara
1307020	1307	Lubuk Basung
1307021	1307	Ampek Nagari
1307030	1307	Tanjung Raya
1307040	1307	Matur
1307050	1307	Iv Koto
1307051	1307	Malalak
1307061	1307	Banuhampu
1307062	1307	Sungai Pua
1307070	1307	Ampek Angkek
1307071	1307	Canduang
1307080	1307	Baso
1307090	1307	Tilatang Kamang
1307091	1307	Kamang Magek
1307100	1307	Palembayan
1307110	1307	Palupuh
1308010	1308	Payakumbuh
1308011	1308	Akabiluru
1308020	1308	Luak
1308021	1308	Lareh Sago Halaban
1308022	1308	Situjuah Limo Nagari
1308030	1308	Harau
1308040	1308	Guguak
1308041	1308	Mungka
1308050	1308	Suliki
1308051	1308	Bukik Barisan
1308060	1308	Gunuang Omeh
1308070	1308	Kapur Ix
1308080	1308	Pangkalan Koto Baru
1309070	1309	Bonjol
1309071	1309	Tigo Nagari
1309072	1309	Simpang Alahan Mati
1309080	1309	Lubuk Sikaping
1309100	1309	Dua Koto
1309110	1309	Panti
1309111	1309	Padang Gelugur
1309121	1309	Rao
1309122	1309	Mapat Tunggul
1309123	1309	Mapat Tunggul Selatan
1309124	1309	Rao Selatan
1309125	1309	Rao Utara
1310010	1310	Sangir
1310020	1310	Sangir Jujuan
1310021	1310	Sangir Balai Janggo
1310030	1310	Sangir Batang Hari
1310040	1310	Sungai Pagu
1310041	1310	Pauah Duo
1310050	1310	Koto Parik Gadang Diateh
1311010	1311	Sungai Rumbai
1311011	1311	Koto Besar
1311012	1311	Asam Jujuhan
1311020	1311	Koto Baru
1311021	1311	Koto Salak
1311022	1311	Tiumang
1311023	1311	Padang Laweh
1311030	1311	Sitiung
1311031	1311	Timpeh
1311040	1311	Pulau Punjung
1311041	1311	Ix Koto
1312010	1312	Sungai Beremas
1312020	1312	Ranah Batahan
1312030	1312	Koto Balingka
1312040	1312	Sungai Aur
1312050	1312	Lembah Malintang
1312060	1312	Gunung Tuleh
1312070	1312	Talamau
1312080	1312	Pasaman
1312090	1312	Luhak Nan Duo
1312100	1312	Sasak Ranah Pasisie
1312110	1312	Kinali
1371010	1371	Bungus Teluk Kabung
1371020	1371	Lubuk Kilangan
1371030	1371	Lubuk Begalung
1371040	1371	Padang Selatan
1371050	1371	Padang Timur
1371060	1371	Padang Barat
1371070	1371	Padang Utara
1371080	1371	Nanggalo
1371090	1371	Kuranji
1371100	1371	Pauh
1371110	1371	Koto Tangah
1372010	1372	Lubuk Sikarah
1372020	1372	Tanjung Harapan
1373010	1373	Silungkang
1373020	1373	Lembah Segar
1373030	1373	Barangin
1373040	1373	Talawi
1374010	1374	Padang Panjang Barat
1374020	1374	Padang Panjang Timur
1375010	1375	Guguk Panjang
1375020	1375	Mandiangin Koto Selayan
1375030	1375	Aur Birugo Tigo Baleh
1376010	1376	Payakumbuh Barat
1376011	1376	Payakumbuh Selatan
1376020	1376	Payakumbuh Timur
1376030	1376	Payakumbuh Utara
1376031	1376	Lamposi Tigo Nagori
1377010	1377	Pariaman Selatan
1377020	1377	Pariaman Tengah
1377021	1377	Pariaman Timur
1377030	1377	Pariaman Utara
1401010	1401	Kuantan Mudik
1401011	1401	Hulu Kuantan
1401012	1401	Gunung Toar
1401013	1401	Pucuk Rantau
1401020	1401	Singingi
1401021	1401	Singingi Hilir
1401030	1401	Kuantan Tengah
1401031	1401	Sentajo Raya
1401040	1401	Benai
1401050	1401	Kuantan Hilir
1401051	1401	Pangean
1401052	1401	Logas Tanah Darat
1401053	1401	Kuantan Hilir Seberang
1401060	1401	Cerenti
1401061	1401	Inuman
1402010	1402	Peranap
1402011	1402	Batang Peranap
1402020	1402	Seberida
1402021	1402	Batang Cenaku
1402022	1402	Batang Gansal
1402030	1402	Kelayang
1402031	1402	Rakit Kulim
1402040	1402	Pasir Penyu
1402041	1402	Lirik
1402042	1402	Sungai Lala
1402043	1402	Lubuk Batu Jaya
1402050	1402	Rengat Barat
1402060	1402	Rengat
1402061	1402	Kuala Cenaku
1403010	1403	Keritang
1403011	1403	Kemuning
1403020	1403	Reteh
1403021	1403	Sungai Batang
1403030	1403	Enok
1403040	1403	Tanah Merah
1403050	1403	Kuala Indragiri
1403051	1403	Concong
1403060	1403	Tembilahan
1403061	1403	Tembilahan Hulu
1403070	1403	Tempuling
1403071	1403	Kempas
1403080	1403	Batang Tuaka
1403090	1403	Gaung Anak Serka
1403100	1403	Gaung
1403110	1403	Mandah
1403120	1403	Kateman
1403121	1403	Pelangiran
1403122	1403	Teluk Belengkong
1403123	1403	Pulau Burung
1404010	1404	Langgam
1404011	1404	Pangkalan Kerinci
1404012	1404	Bandar Seikijang
1404020	1404	Pangkalan Kuras
1404021	1404	Ukui
1404022	1404	Pangkalan Lesung
1404030	1404	Bunut
1404031	1404	Pelalawan
1404032	1404	Bandar Petalangan
1404040	1404	Kuala Kampar
1404041	1404	Kerumutan
1404042	1404	Teluk Meranti
1405010	1405	Minas
1405011	1405	Sungai Mandau
1405012	1405	Kandis
1405020	1405	Siak
1405021	1405	Kerinci Kanan
1405022	1405	Tualang
1405023	1405	Dayun
1405024	1405	Lubuk Dalam
1405025	1405	Koto Gasib
1405026	1405	Mempura
1405030	1405	Sungai Apit
1405031	1405	Bunga Raya
1405032	1405	Sabak Auh
1405033	1405	Pusako
1406010	1406	Kampar Kiri
1406011	1406	Kampar Kiri Hulu
1406012	1406	Kampar Kiri Hilir
1406013	1406	Gunung Sahilan
1406014	1406	Kampar Kiri Tengah
1406020	1406	Xiii Koto Kampar
1406021	1406	Koto Kampar Hulu
1406030	1406	Kuok
1406031	1406	Salo
1406040	1406	Tapung
1406041	1406	Tapung Hulu
1406042	1406	Tapung Hilir
1406050	1406	Bangkinang city
1406051	1406	Bangkinang
1406060	1406	Kampar
1406061	1406	Kampa
1406062	1406	Rumbio Jaya
1406063	1406	Kampar Utara
1406070	1406	Tambang
1406080	1406	Siak Hulu
1406081	1406	Perhentian Raja
1407010	1407	Rokan Iv Koto
1407011	1407	Pendalian Iv Koto
1407020	1407	Tandun
1407021	1407	Kabun
1407022	1407	Ujung Batu
1407030	1407	Rambah Samo
1407040	1407	Rambah
1407041	1407	Rambah Hilir
1407042	1407	Bangun Purba
1407050	1407	Tambusai
1407051	1407	Tambusai Utara
1407060	1407	Kepenuhan
1407061	1407	Kepenuhan Hulu
1407070	1407	Kunto Darussalam
1407071	1407	Pagaran Tapah Darussalam
1407072	1407	Bonai Darussalam
1408010	1408	Mandau
1408011	1408	Pinggir
1408012	1408	Bathin Solapan
1408013	1408	Talang Muandau
1408020	1408	Bukit Batu
1408021	1408	Siak Kecil
1408022	1408	Bandar Laksamana
1408030	1408	Rupat
1408031	1408	Rupat Utara
1408040	1408	Bengkalis
1408050	1408	Bantan
1409010	1409	Tanah Putih
1409011	1409	Pujud
1409012	1409	Tanah Putih Tanjung Melawan
1409013	1409	Rantau Kopar
1409014	1409	Tanjung Medan
1409020	1409	Bagan Sinembah
1409021	1409	Simpang Kanan
1409022	1409	Bagan Sinembah Raya
1409023	1409	Balai Jaya
1409030	1409	Kubu
1409031	1409	Pasir Limau Kapas
1409032	1409	Kubu Babussalam
1409040	1409	Bangko
1409041	1409	Sinaboi
1409042	1409	Batu Hampar
1409043	1409	Pekaitan
1409050	1409	Rimba Melintang
1409051	1409	Bangko Pusako
1410010	1410	Tebing Tinggi Barat
1410020	1410	Tebing Tinggi
1410021	1410	Tebing Tinggi Timur
1410030	1410	Rangsang
1410031	1410	Rangsang Pesisir
1410040	1410	Rangsang Barat
1410050	1410	Merbau
1410051	1410	Pulau Merbau
1410052	1410	Tasik Putri Puyu
1471010	1471	Tampan
1471011	1471	Payung Sekaki
1471020	1471	Bukit Raya
1471021	1471	Marpoyan Damai
1471022	1471	Tenayan Raya
1471030	1471	Limapuluh
1471040	1471	Sail
1471050	1471	Pekanbaru city
1471060	1471	Sukajadi
1471070	1471	Senapelan
1471080	1471	Rumbai
1471081	1471	Rumbai Pesisir
1473010	1473	Bukit Kapur
1473011	1473	Medang Kampai
1473012	1473	Sungai Sembilan
1473020	1473	Dumai Barat
1473021	1473	Dumai Selatan
1473030	1473	Dumai Timur
1473031	1473	Dumai city
1501010	1501	Gunung Raya
1501011	1501	Bukit Kerman
1501020	1501	Batang Merangin
1501030	1501	Keliling Danau
1501040	1501	Danau Kerinci
1501050	1501	Sitinjau Laut
1501070	1501	Air Hangat
1501071	1501	Air Hangat Timur
1501072	1501	Depati Vii
1501073	1501	Air Hangat Barat
1501080	1501	Gunung Kerinci
1501081	1501	Siulak
1501082	1501	Siulak Mukai
1501090	1501	Kayu Aro
1501091	1501	Gunung Tujuh
1501092	1501	Kayu Aro Barat
1502010	1502	Jangkat
1502011	1502	Sungai Tenang
1502020	1502	Muara Siau
1502021	1502	Lembah Masurai
1502022	1502	Tiang Pumpung
1502030	1502	Pamenang
1502031	1502	Pamenang Barat
1502032	1502	Renah Pamenang
1502033	1502	Pamenang Selatan
1502040	1502	Bangko
1502041	1502	Bangko Barat
1502042	1502	Nalo Tantan
1502043	1502	Batang Masumai
1502050	1502	Sungai Manau
1502051	1502	Renah Pembarap
1502052	1502	Pangkalan timebu
1502060	1502	Tabir
1502061	1502	Tabir Ulu
1502062	1502	Tabir Selatan
1502063	1502	Tabir Ilir
1502064	1502	Tabir Timur
1502065	1502	Tabir Lintas
1502066	1502	Margo Tabir
1502067	1502	Tabir Barat
1503010	1503	Batang Asai
1503020	1503	Limun
1503021	1503	Cermin Nan Gedang
1503030	1503	Pelawan
1503031	1503	Singkut
1503040	1503	Sarolangun
1503041	1503	Bathin Viii
1503050	1503	Pauh
1503051	1503	Air Hitam
1503060	1503	Mandiangin
1504010	1504	Mersam
1504011	1504	Maro Sebo Ulu
1504020	1504	Batin Xxiv
1504030	1504	Muara Tembesi
1504040	1504	Muara Bulian
1504041	1504	Bajubang
1504042	1504	Maro Sebo Ilir
1504050	1504	Pemayung
1505010	1505	Mestong
1505011	1505	Sungai Bahar
1505012	1505	Bahar Selatan
1505013	1505	Bahar Utara
1505020	1505	Kumpeh Ulu
1505021	1505	Sungai Gelam
1505030	1505	Kumpeh
1505040	1505	Maro Sebo
1505041	1505	Taman Rajo
1505050	1505	timebi Luar city
1505060	1505	Sekernan
1506010	1506	Mendahara
1506011	1506	Mendahara Ulu
1506012	1506	Geragai
1506020	1506	Dendang
1506031	1506	Muara Sabak Barat
1506032	1506	Muara Sabak Timur
1506033	1506	Kuala timebi
1506040	1506	Rantau Rasau
1506041	1506	Berbak
1506050	1506	Nipah Panjang
1506060	1506	Sadu
1507010	1507	Tungkal Ulu
1507011	1507	Merlung
1507012	1507	Batang Asam
1507013	1507	Tebing Tinggi
1507014	1507	Renah Mendaluh
1507015	1507	Muara Papalik
1507020	1507	Pengabuan
1507021	1507	Senyerang
1507030	1507	Tungkal Ilir
1507031	1507	Bram Itam
1507032	1507	Seberang city
1507040	1507	Betara
1507041	1507	Kuala Betara
1508010	1508	Tebo Ilir
1508011	1508	Muara Tabir
1508020	1508	Tebo Tengah
1508021	1508	Sumay
1508022	1508	Tengah Ilir
1508030	1508	Rimbo Bujang
1508031	1508	Rimbo Ulu
1508032	1508	Rimbo Ilir
1508040	1508	Tebo Ulu
1508041	1508	Vii Koto
1508042	1508	Serai Serumpun
1508043	1508	Vii Koto Ilir
1509010	1509	Pelepat
1509011	1509	Pelepat Ilir
1509021	1509	Bathin Ii Babeko
1509022	1509	Rimbo Tengah
1509023	1509	Bungo Dani
1509024	1509	Pasar Muara Bungo
1509025	1509	Bathin Iii
1509030	1509	Rantau Pandan
1509031	1509	Muko-Muko Bathin Vii
1509032	1509	Bathin Iii Ulu
1509040	1509	Tanah Sepenggal
1509041	1509	Tanah Sepenggal Lintas
1509050	1509	Tanah Tumbuh
1509051	1509	Limbur Lubuk Mengkuang
1509052	1509	Bathin Ii Pelayang
1509060	1509	Jujuhan
1509061	1509	Jujuhan Ilir
1571010	1571	city Baru
1571011	1571	Alam Barajo
1571020	1571	timebi Selatan
1571021	1571	Paal Merah
1571030	1571	Jelutung
1571040	1571	Pasar timebi
1571050	1571	Telanaipura
1571051	1571	Danau Sipin
1571060	1571	Danau Teluk
1571070	1571	Pelayangan
1571080	1571	timebi Timur
1572010	1572	Tanah Kampung
1572020	1572	Kumun Debai
1572030	1572	Sungai Penuh
1572031	1572	Pondok Tinggi
1572032	1572	Sungai Bungkal
1572040	1572	Hamparan Rawang
1572050	1572	Pesisir Bukit
1572051	1572	Koto Baru
1601052	1601	Lengkiti
1601070	1601	Sosoh Buay Rayap
1601080	1601	Pengandonan
1601081	1601	Semidang Aji
1601082	1601	Ulu Ogan
1601083	1601	Muara Jaya
1601090	1601	Peninjauan
1601091	1601	Lubuk Batang
1601092	1601	Sinar Peninjauan
1601093	1601	Kedaton Peninjauan Raya
1601130	1601	Batu Raja Timur
1601131	1601	Lubuk Raja
1601140	1601	Batu Raja Barat
1602010	1602	Lempuing
1602011	1602	Lempuing Jaya
1602020	1602	Mesuji
1602021	1602	Sungai Menang
1602022	1602	Mesuji Makmur
1602023	1602	Mesuji Raya
1602030	1602	Tulung Selapan
1602031	1602	Cengal
1602040	1602	Pedamaran
1602041	1602	Pedamaran Timur
1602050	1602	Tanjung Lubuk
1602051	1602	Teluk Gelam
1602060	1602	city Kayu Agung
1602120	1602	Sirah Pulau Padang
1602121	1602	Jejawi
1602130	1602	Pampangan
1602131	1602	Pangkalan Lapam
1602140	1602	Air Sugihan
1603010	1603	Semendo Darat Laut
1603011	1603	Semendo Darat Ulu
1603012	1603	Semendo Darat Tengah
1603020	1603	Tanjung Agung
1603031	1603	Rambang
1603032	1603	Lubai
1603033	1603	Lubai Ulu
1603040	1603	Lawang Kidul
1603050	1603	Muara Enim
1603051	1603	Ujan Mas
1603060	1603	Gunung Megang
1603061	1603	Benakat
1603062	1603	Belimbing
1603070	1603	Rambang Dangku
1603090	1603	Gelumbang
1603091	1603	Lembak
1603092	1603	Sungai Rotan
1603093	1603	Muara Belida
1603094	1603	Kelekar
1603095	1603	Belida Darat
1604011	1604	Tanjung Sakti Pumi
1604012	1604	Tanjung Sakti Pumu
1604040	1604	city Agung
1604041	1604	Mulak Ulu
1604042	1604	Tanjung Tebat
1604043	1604	Mulak Sebingkai
1604050	1604	Pulau Pinang
1604051	1604	Pagar Gunung
1604052	1604	Gumay Ulu
1604060	1604	Jarai
1604061	1604	Pajar month
1604062	1604	Muara Payang
1604063	1604	Sukamerindu
1604111	1604	Kikim Barat
1604112	1604	Kikim Timur
1604113	1604	Kikim Selatan
1604114	1604	Kikim Tengah
1604120	1604	Lahat
1604121	1604	Gumay Talang
1604122	1604	Pseksu
1604123	1604	Lahat Selatan
1604131	1604	Merapi Barat
1604132	1604	Merapi Timur
1604133	1604	Merapi Selatan
1605030	1605	Suku Tengah Lakitan Ulu
1605031	1605	Selangit
1605032	1605	Sumber Harta
1605040	1605	Tugumulyo
1605041	1605	Purwodadi
1605050	1605	Muara Beliti
1605051	1605	Tiang Pumpung Kepungut
1605060	1605	Jayaloka
1605061	1605	Suka Karya
1605070	1605	Muara Kelingi
1605071	1605	monthg Tengah Suku Ulu
1605072	1605	Tuah Negeri
1605080	1605	Muara Lakitan
1605090	1605	Megang Sakti
1606010	1606	Sanga country
1606020	1606	Babat Toman
1606021	1606	Batanghari Leko
1606022	1606	Plakat Tinggi
1606023	1606	Lawang Wetan
1606030	1606	Sungai Keruh
1606040	1606	Sekayu
1606041	1606	Lais
1606090	1606	Sungai Lilin
1606091	1606	Keluang
1606092	1606	Babat Supat
1606100	1606	Bayung Lencir
1606101	1606	Lalan
1606102	1606	Tungkal Jaya
1607010	1607	Rantau Bayur
1607020	1607	Betung
1607021	1607	Suak Tapeh
1607030	1607	Pulau Rimau
1607031	1607	Tungkal Ilir
1607040	1607	Banyuasin Iii
1607041	1607	Sembawa
1607050	1607	Talang Kelapa
1607051	1607	Tanjung Lago
1607060	1607	Banyuasin I
1607061	1607	Air Kumbang
1607070	1607	Rambutan
1607080	1607	Muara Padang
1607081	1607	Muara Sugihan
1607090	1607	Makarti Jaya
1607091	1607	Air Saleh
1607100	1607	Banyuasin Ii
1607110	1607	Muara Telang
1607111	1607	Sumber Marga Telang
1608010	1608	Mekakau Ilir
1608020	1608	Banding Agung
1608021	1608	Warkuk Ranau Selatan
1608022	1608	Buay Pematang Ribu Ranau Tengah
1608030	1608	Buay Pemaca
1608040	1608	Simpang
1608041	1608	Buana Pemaca
1608050	1608	Muaradua
1608051	1608	Buay Rawan
1608060	1608	Buay Sandang Aji
1608061	1608	Tiga Dihaji
1608070	1608	Buay Runjung
1608071	1608	Runjung Agung
1608080	1608	Kisam Tinggi
1608090	1608	Muaradua Kisam
1608091	1608	Kisam Ilir
1608100	1608	Pulau Beringin
1608101	1608	Sindang Danau
1608102	1608	Sungai Are
1609010	1609	Martapura
1609011	1609	Bunga Mayang
1609012	1609	Jaya Pura
1609020	1609	Buay Pemuka Peliung
1609030	1609	Buay Madang
1609031	1609	Buay Madang Timur
1609032	1609	Buay Pemuka Bangsa Raja
1609040	1609	Madang Suku Ii
1609041	1609	Madang Suku Iii
1609050	1609	Madang Suku I
1609051	1609	Belitang Madang Raya
1609060	1609	Belitang
1609061	1609	Belitang Jaya
1609070	1609	Belitang Iii
1609080	1609	Belitang Ii
1609081	1609	Belitang Mulya
1609090	1609	Semendawai Suku Iii
1609091	1609	Semendawai Timur
1609100	1609	Cempaka
1609101	1609	Semendawai Barat
1610010	1610	Muara Kuang
1610011	1610	Rambang Kuang
1610012	1610	Lubuk Keliat
1610020	1610	Tanjung Batu
1610021	1610	Payaraman
1610030	1610	Rantau Alai
1610031	1610	Kandis
1610040	1610	Tanjung Raja
1610041	1610	Rantau Panjang
1610042	1610	Sungai Pinang
1610050	1610	Pemulutan
1610051	1610	Pemulutan Selatan
1610052	1610	Pemulutan Barat
1610060	1610	Indralaya
1610061	1610	Indralaya Utara
1610062	1610	Indralaya Selatan
1611010	1611	Muara Pinang
1611020	1611	Lintang Kanan
1611030	1611	Pendopo
1611031	1611	Pendopo Barat
1611040	1611	Pasemah Air Keruh
1611050	1611	Ulu Musi
1611051	1611	Sikap Dalam
1611060	1611	Talang Padang
1611070	1611	Tebing Tinggi
1611071	1611	Saling
1612010	1612	Talang Ubi
1612020	1612	Tanah Abang
1612030	1612	Abab
1612040	1612	Penukal
1612050	1612	Penukal Utara
1613010	1613	Ulu Rawas
1613020	1613	Karang Jaya
1613030	1613	Rawas Ulu
1613040	1613	Rupit
1613050	1613	Karang Dapo
1613060	1613	Rawas Ilir
1613070	1613	Nibung
1671010	1671	Ilir Barat Ii
1671011	1671	Gandus
1671020	1671	Seberang Ulu I
1671021	1671	Kertapati
1671022	1671	Jakabaring
1671030	1671	Seberang Ulu Ii
1671031	1671	Plaju
1671040	1671	Ilir Barat I
1671041	1671	Bukit Kecil
1671050	1671	Ilir Timur I
1671051	1671	Kemuning
1671060	1671	Ilir Timur Ii
1671061	1671	Kalidoni
1671062	1671	Ilir Timur Iii
1671070	1671	Sako
1671071	1671	Sematang Borang
1671080	1671	Sukarami
1671081	1671	Alang Alang Lebar
1672010	1672	Rambang Kapak Tengah
1672020	1672	Prabumulih Timur
1672021	1672	Prabumulih Selatan
1672030	1672	Prabumulih Barat
1672031	1672	Prabumulih Utara
1672040	1672	Cambai
1673010	1673	Dempo Selatan
1673011	1673	Dempo Tengah
1673020	1673	Dempo Utara
1673030	1673	Pagar Alam Selatan
1673040	1673	Pagar Alam Utara
1674011	1674	Lubuk Linggau Barat I
1674012	1674	Lubuk Linggau Barat Ii
1674021	1674	Lubuk Linggau Selatan I
1674022	1674	Lubuk Linggau Selatan Ii
1674031	1674	Lubuk Linggau Timur I
1674032	1674	Lubuk Linggau Timur Ii
1674041	1674	Lubuk Linggau Utara I
1674042	1674	Lubuk Linggau Utara Ii
1701040	1701	Manna
1701041	1701	city Manna
1701042	1701	Kedurang
1701043	1701	Bunga Mas
1701044	1701	Pasar Manna
1701045	1701	Kedurang Ilir
1701050	1701	Seginim
1701051	1701	Air Nipis
1701060	1701	Pino
1701061	1701	Pinoraya
1701062	1701	Ulu Manna
1702020	1702	city Padang
1702021	1702	Sindang Beliti Ilir
1702030	1702	Padang Ulak Tanding
1702031	1702	Sindang Kelingi
1702032	1702	Bindu Riang
1702033	1702	Sindang Beliti Ulu
1702034	1702	Sindang Dataran
1702040	1702	Curup
1702041	1702	Bermani Ulu
1702042	1702	Selupu Rejang
1702043	1702	Curup Selatan
1702044	1702	Curup Tengah
1702045	1702	Bermani Ulu Raya
1702046	1702	Curup Utara
1702047	1702	Curup Timur
1703010	1703	Enggano
1703050	1703	Kerkap
1703051	1703	Air Napal
1703052	1703	Air Besi
1703053	1703	Hulu Palik
1703054	1703	Tanjung Agung Palik
1703060	1703	Arga Makmur
1703061	1703	Arma Jaya
1703070	1703	Lais
1703071	1703	Batik Nau
1703072	1703	Giri Mulya
1703073	1703	Air Padang
1703080	1703	Padang Jaya
1703090	1703	Keyear
1703091	1703	Napal Putih
1703092	1703	Ulok Kupai
1703093	1703	Pinang Raya
1703100	1703	Putri Hijau
1703101	1703	Marga Sakti Sebelat
1704010	1704	Nasal
1704020	1704	Maje
1704030	1704	Kaur Selatan
1704031	1704	Tetap
1704040	1704	Kaur Tengah
1704041	1704	Luas
1704042	1704	Muara Sahung
1704050	1704	Kinal
1704051	1704	Semidang Gumay
1704060	1704	Tanjung Kemuning
1704061	1704	Kelam Tengah
1704070	1704	Kaur Utara
1704071	1704	Padang Guci Hilir
1704072	1704	Lungkang Kule
1704073	1704	Padang Guci Hulu
1705010	1705	Semidang Alas Maras
1705020	1705	Semidang Alas
1705030	1705	Talo
1705031	1705	Ilir Talo
1705032	1705	Talo Kecil
1705033	1705	Ulu Talo
1705040	1705	Seluma
1705041	1705	Seluma Selatan
1705042	1705	Seluma Barat
1705043	1705	Seluma Timur
1705044	1705	Seluma Utara
1705050	1705	Sukaraja
1705051	1705	Air Periukan
1705052	1705	Lubuk Sandi
1706010	1706	Ipuh
1706011	1706	Air Rami
1706012	1706	Malin Deman
1706020	1706	Pondok Suguh
1706021	1706	Sungai Rumbai
1706022	1706	Teramang Jaya
1706030	1706	Teras Teruntime
1706031	1706	Penarik
1706032	1706	Selagan Raya
1706040	1706	city Mukomuko
1706041	1706	Air Dikit
1706042	1706	Xiv Koto
1706050	1706	Lubuk Pinang
1706051	1706	Air Manjunto
1706052	1706	V Koto
1707010	1707	Rimbo Pengadang
1707011	1707	Topos
1707020	1707	Lebong Selatan
1707021	1707	Bingin Kuning
1707030	1707	Lebong Tengah
1707031	1707	Lebong Sakti
1707040	1707	Lebong Atas
1707042	1707	Pelabai
1707050	1707	Lebong Utara
1707051	1707	Amen
1707052	1707	Uram Jaya
1707053	1707	Pinang Belapis
1708010	1708	Muara Kemumu
1708020	1708	Bermani Ilir
1708030	1708	Seberang Musi
1708040	1708	Tebat Karai
1708050	1708	Kepahiang
1708060	1708	Kaba Wetan
1708070	1708	Ujan Mas
1708080	1708	Merigi
1709010	1709	Talang Empat
1709020	1709	Karang Tinggi
1709030	1709	Taba Penanjung
1709031	1709	Merigi Kelindang
1709040	1709	Pagar Jati
1709041	1709	Merigi Sakti
1709050	1709	Pondok Kelapa
1709051	1709	Pondok Kubang
1709060	1709	Pematang Tiga
1709061	1709	Bang Haji
1771010	1771	Selebar
1771011	1771	Kampung Melayu
1771020	1771	Gading Cempaka
1771021	1771	Ratu Agung
1771022	1771	Ratu Samban
1771023	1771	Singaran Pati
1771030	1771	Teluk Segara
1771031	1771	Sungai Serut
1771040	1771	Muara Bangka Hulu
1801040	1801	Balik Bukit
1801041	1801	Sukau
1801042	1801	Lumbok Seminung
1801050	1801	Belalau
1801051	1801	Sekincau
1801052	1801	Suoh
1801053	1801	Batu Brak
1801054	1801	Pagar Dewa
1801055	1801	Batu Ketulis
1801056	1801	Bandar Negeri Suoh
1801060	1801	Sumber Jaya
1801061	1801	Way Tenong
1801062	1801	Gedung Surian
1801063	1801	Kebun Tebu
1801064	1801	Air Hitam
1802010	1802	Wonosobo
1802011	1802	Semaka
1802012	1802	Bandar Negeri Semuong
1802020	1802	city Agung
1802021	1802	Pematang Sawa
1802022	1802	city Agung Timur
1802023	1802	city Agung Barat
1802030	1802	Pulau Panggung
1802031	1802	Ulubelu
1802032	1802	Air Naningan
1802040	1802	Talang Padang
1802041	1802	Sumberejo
1802042	1802	Gisting
1802043	1802	Gunung Alip
1802050	1802	Pugung
1802101	1802	Bulok
1802110	1802	Cukuh Balak
1802111	1802	Kelumbayan
1802112	1802	Limau
1802113	1802	Kelumbayan Barat
1803060	1803	Natar
1803070	1803	Jati Agung
1803080	1803	Tanjung Bintang
1803081	1803	Tanjung Sari
1803090	1803	Katibung
1803091	1803	Merbau Mataram
1803092	1803	Way Sulan
1803100	1803	Sidomulyo
1803101	1803	Candipuro
1803102	1803	Way Panji
1803110	1803	Kalianda
1803111	1803	Rajabasa
1803120	1803	Palas
1803121	1803	Sragi
1803130	1803	Penengahan
1803131	1803	Ketapang
1803132	1803	Bakauheni
1804010	1804	Metro Kibang
1804020	1804	Batanghari
1804030	1804	Sekampung
1804040	1804	Margatiga
1804050	1804	Sekampung Udik
1804060	1804	Jabung
1804061	1804	Pasir Sakti
1804062	1804	Waway Karya
1804063	1804	Marga Sekampung
1804070	1804	Labuhan Maringgai
1804071	1804	Mataram Baru
1804072	1804	Bandar Sribawono
1804073	1804	Melinting
1804074	1804	Gunung Pelindung
1804080	1804	Way Jepara
1804081	1804	Braja Slebah
1804082	1804	Labuhan Ratu
1804090	1804	Sukadana
1804091	1804	Bumi Agung
1804092	1804	Batanghari Nuban
1804100	1804	Pekalongan
1804110	1804	Raman Utara
1804120	1804	Purbolinggo
1804121	1804	Way Bungur
1805010	1805	Padang Ratu
1805011	1805	Selagai Lingga
1805012	1805	Pubian
1805013	1805	Anak Tuha
1805014	1805	Anak Ratu Aji
1805020	1805	Kalirejo
1805021	1805	Sendang Agung
1805030	1805	Bangunrejo
1805040	1805	Gunung Sugih
1805041	1805	Bekri
1805042	1805	Bumi Ratu Nuban
1805050	1805	Trimurjo
1805060	1805	Punggur
1805061	1805	city Gajah
1805070	1805	Seputih Raman
1805080	1805	Terbanggi Besar
1805081	1805	Seputih Agung
1805082	1805	Way Pengubuan
1805090	1805	Terusan Nunyai
1805100	1805	Seputih Mataram
1805101	1805	Bandar Mataram
1805110	1805	Seputih Banyak
1805111	1805	Way Seputih
1805120	1805	Rumbia
1805121	1805	Bumi Nabung
1805122	1805	Putra Rumbia
1805130	1805	Seputih Surabaya
1805131	1805	Bandar Surabaya
1806010	1806	Bukit Kemuning
1806011	1806	Abung Tinggi
1806020	1806	Tanjung Raja
1806030	1806	Abung Barat
1806031	1806	Abung Tengah
1806032	1806	Abung  Kunang
1806033	1806	Abung Pekurun
1806040	1806	citybumi
1806041	1806	citybumi Utara
1806042	1806	citybumi Selatan
1806050	1806	Abung Selatan
1806051	1806	Abung Semuli
1806052	1806	Blambangan Pagar
1806060	1806	Abung Timur
1806061	1806	Abung Surakarta
1806070	1806	Sungkai Selatan
1806071	1806	Muara Sungkai
1806072	1806	Bunga Mayang
1806073	1806	Sungkai  Barat
1806074	1806	Sungkai Jaya
1806080	1806	Sungkai Utara
1806081	1806	Hulusungkai
1806082	1806	Sungkai Tengah
1807010	1807	Banjit
1807020	1807	Baradatu
1807021	1807	Gunung Labuhan
1807030	1807	Kasui
1807031	1807	Rebang Tangkas
1807040	1807	Blambangan Umpu
1807041	1807	Way Tuba
1807042	1807	Negeri Agung
1807050	1807	Bahuga
1807051	1807	Buay  Bahuga
1807052	1807	Bumi Agung
1807060	1807	Pakuan Ratu
1807061	1807	Negara Batin
1807062	1807	Negeri Besar
1808030	1808	Banjar Agung
1808031	1808	Banjar Margo
1808032	1808	Banjar Baru
1808040	1808	Gedung Aji
1808041	1808	Penawar Aji
1808042	1808	Meraksa Aji
1808050	1808	Menggala
1808051	1808	Penawar Tama
1808052	1808	Rawajitu Selatan
1808053	1808	Gedung Meneng
1808054	1808	Rawajitu Timur
1808055	1808	Rawa Pitu
1808056	1808	Gedung Aji Baru
1808057	1808	Dente Teladas
1808058	1808	Menggala Timur
1809010	1809	Punduh Pidada
1809011	1809	Marga Punduh
1809020	1809	Padang Cermin
1809021	1809	Teluk Pandan
1809022	1809	Way Ratai
1809030	1809	Kedondong
1809031	1809	Way Khilau
1809040	1809	Way Lima
1809050	1809	Gedung Tataan
1809060	1809	Negeri Katon
1809070	1809	Tegineneng
1810010	1810	Pardasuka
1810020	1810	Ambarawa
1810030	1810	Pagelaran
1810031	1810	Pagelaran Utara
1810040	1810	Pringsewu
1810050	1810	Gading Rejo
1810060	1810	Sukoharjo
1810070	1810	Banyumas
1810080	1810	Adi Luwih
1811010	1811	Way Serdang
1811020	1811	Simpang Pematang
1811030	1811	Panca Jaya
1811040	1811	Tanjung Raya
1811050	1811	Mesuji
1811060	1811	Mesuji Timur
1811070	1811	Rawajitu Utara
1812010	1812	Tulang Bawang Udik
1812020	1812	Tumi Jajar
1812030	1812	Tulang Bawang Tengah
1812040	1812	Pagar Dewa
1812050	1812	Lambu Kibang
1812060	1812	Gunung Terang
1812061	1812	Batu Putih
1812070	1812	Gunung Agung
1812080	1812	Way Kenanga
1813010	1813	Lemong
1813020	1813	Pesisir Utara
1813030	1813	Pulau Pisang
1813040	1813	Karya Penggawa
1813050	1813	Way Krui
1813060	1813	Pesisir Tengah
1813070	1813	Krui Selatan
1813080	1813	Pesisir Selatan
1813090	1813	Ngambur
1813100	1813	Bengkunat
1813110	1813	Bengkunat Belimbing
1871010	1871	Teluk Betung Barat
1871011	1871	Telukbetung Timur
1871020	1871	Teluk Betung Selatan
1871021	1871	Bumi Waras
1871030	1871	Panjang
1871040	1871	Tanjung Karang Timur
1871041	1871	Kedamaian
1871050	1871	Teluk Betung Utara
1871060	1871	Tanjung Karang Pusat
1871061	1871	Enggal
1871070	1871	Tanjung Karang Barat
1871071	1871	Kemiling
1871072	1871	Langkapura
1871080	1871	Kedaton
1871081	1871	Rajabasa
1871082	1871	Tanjung Senang
1871083	1871	Labuhan Ratu
1871090	1871	Sukarame
1871091	1871	Sukabumi
1871092	1871	Way Halim
1872011	1872	Metro Selatan
1872012	1872	Metro Barat
1872021	1872	Metro Timur
1872022	1872	Metro Pusat
1872023	1872	Metro Utara
1901070	1901	Mendo Barat
1901080	1901	Merawang
1901081	1901	Puding Besar
1901090	1901	Sungai Liat
1901091	1901	Pemali
1901092	1901	Bakam
1901130	1901	Belinyu
1901131	1901	Riau Silip
1902010	1902	Membalong
1902060	1902	Tanjung Pandan
1902061	1902	Badau
1902062	1902	Sijuk
1902063	1902	Selat Nasik
1903010	1903	Kelapa
1903020	1903	Tempilang
1903030	1903	Mentok
1903040	1903	Simpang Teritip
1903050	1903	Jebus
1903051	1903	Parittiga
1904010	1904	Koba
1904011	1904	Lubuk Besar
1904020	1904	Pangkalan Baru
1904021	1904	nameng
1904030	1904	Sungai Selan
1904040	1904	Simpang Katis
1905010	1905	Payung
1905011	1905	Pulau Besar
1905020	1905	Simpang Rimba
1905030	1905	Toboali
1905031	1905	Tukak Sadai
1905040	1905	Air Gegas
1905050	1905	Lepar Pongok
1905051	1905	Kepulauan Pongok
1906010	1906	Dendang
1906011	1906	Simpang Pesak
1906020	1906	Gantung
1906021	1906	Simpang Renggiang
1906030	1906	Manggar
1906031	1906	Damar
1906040	1906	Kelapa Kampit
1971010	1971	Rangkui
1971020	1971	Bukit Intan
1971021	1971	Girimaya
1971030	1971	Pangkal Balam
1971031	1971	Gabek
1971040	1971	Taman Sari
1971041	1971	Gerunggang
2101010	2101	Moro
2101011	2101	Durai
2101020	2101	Kundur
2101021	2101	Kundur Utara
2101022	2101	Kundur Barat
2101023	2101	Ungar
2101024	2101	Belat
2101030	2101	Karimun
2101031	2101	Buru
2101032	2101	Meral
2101033	2101	Tebing
2101034	2101	Meral Barat
2102040	2102	Teluk Bintan
2102050	2102	Bintan Utara
2102051	2102	Teluk Sebong
2102052	2102	Seri Kuala Lobam
2102060	2102	Bintan Timur
2102061	2102	Gunung Kijang
2102062	2102	Mantang
2102063	2102	Bintan Pesisir
2102064	2102	Toapaya
2102070	2102	Tambelan
2103030	2103	Midai
2103031	2103	Suak Midai
2103040	2103	Bunguran Barat
2103041	2103	Bunguran Utara
2103042	2103	Pulau Laut
2103043	2103	Pulau Tiga
2103044	2103	Bunguran Batubi
2103045	2103	Pulau Tiga Barat
2103050	2103	Bunguran Timur
2103051	2103	Bunguran Timur Laut
2103052	2103	Bunguran Tengah
2103053	2103	Bunguran Selatan
2103060	2103	Serasan
2103061	2103	Subi
2103062	2103	Serasan Timur
2104010	2104	Singkep Barat
2104011	2104	Kepulauan Posek
2104020	2104	Singkep
2104021	2104	Singkep Selatan
2104022	2104	Singkep Pesisir
2104030	2104	Lingga
2104031	2104	Selayar
2104032	2104	Lingga Timur
2104040	2104	Lingga Utara
2104050	2104	Senayang
2105010	2105	Jemaja
2105020	2105	Jemaja Timur
2105030	2105	Siantan Selatan
2105040	2105	Siantan
2105050	2105	Siantan Timur
2105060	2105	Siantan Tengah
2105070	2105	Palmatak
2171010	2171	Belakang Padang
2171020	2171	monthg
2171030	2171	Galang
2171040	2171	Sei Beduk
2171041	2171	Sagulung
2171050	2171	Nongsa
2171051	2171	Batam city
2171060	2171	Sekupang
2171061	2171	Batu Aji
2171070	2171	Lubuk Baja
2171080	2171	Batu Ampar
2171081	2171	Bengkong
2172010	2172	Bukit Bestari
2172020	2172	Tanjungpinang Timur
2172030	2172	Tanjungpinang city
2172040	2172	Tanjungpinang Barat
3101010	3101	Kepulauan Seribu Selatan
3101020	3101	Kepulauan Seribu Utara
3171010	3171	Jagakarsa
3171020	3171	Pasar Minggu
3171030	3171	Cilandak
3171040	3171	Pesanggrahan
3171050	3171	Kebayoran Lama
3171060	3171	Kebayoran Baru
3171070	3171	Mampang Prapatan
3171080	3171	Pancoran
3171090	3171	Tebet
3171100	3171	Setia Budi
3172010	3172	Pasar Rebo
3172020	3172	Ciracas
3172030	3172	Cipayung
3172040	3172	Makasar
3172050	3172	Kramat Jati
3172060	3172	Jatinegara
3172070	3172	Duren Sawit
3172080	3172	Cakung
3172090	3172	Pulo Gadung
3172100	3172	Matraman
3173010	3173	Tanah Abang
3173020	3173	Menteng
3173030	3173	Senen
3173040	3173	Johar Baru
3173050	3173	Cempaka Putih
3173060	3173	Kemayoran
3173070	3173	Sawah Besar
3173080	3173	Gambir
3174010	3174	Kembangan
3174020	3174	Kebon Jeruk
3174030	3174	Palmerah
3174040	3174	Grogol Petamburan
3174050	3174	Tambora
3174060	3174	Taman Sari
3174070	3174	Cengkareng
3174080	3174	Kali Deres
3175010	3175	Penjaringan
3175020	3175	Pademangan
3175030	3175	Tanjung Priok
3175040	3175	Koja
3175050	3175	Kelapa Gading
3175060	3175	Cilincing
3201010	3201	Nanggung
3201020	3201	Leuwiliang
3201021	3201	Leuwisadeng
3201030	3201	Pamijahan
3201040	3201	Cibungmonthg
3201050	3201	Ciampea
3201051	3201	Tenjolaya
3201060	3201	Dramaga
3201070	3201	Ciomas
3201071	3201	Tamansari
3201080	3201	Cijeruk
3201081	3201	Cigombong
3201090	3201	Caringin
3201100	3201	Ciawi
3201110	3201	Cisarua
3201120	3201	Megamendung
3201130	3201	Sukaraja
3201140	3201	Babakan Madang
3201150	3201	Sukamakmur
3201160	3201	Cariu
3201161	3201	Tanjungsari
3201170	3201	Jonggol
3201180	3201	Cileungsi
3201181	3201	Kelapa Nunggal
3201190	3201	Gunung Putri
3201200	3201	Citeureup
3201210	3201	Cibinong
3201220	3201	Bojong Gede
3201221	3201	Tajur Halang
3201230	3201	Kemang
3201231	3201	Ranca Bungur
3201240	3201	Parung
3201241	3201	Ciseeng
3201250	3201	Gunung Sindur
3201260	3201	Rumpin
3201270	3201	Cigudeg
3201271	3201	Sukajaya
3201280	3201	Jasinga
3201290	3201	Tenjo
3201300	3201	Parung Panjang
3202010	3202	Ciemas
3202020	3202	Ciracap
3202021	3202	Waluran
3202030	3202	Surade
3202031	3202	Cibitung
3202040	3202	timepang Kulon
3202041	3202	Cimanggu
3202050	3202	Kali Bunder
3202060	3202	Tegal Buleud
3202070	3202	Cidolog
3202080	3202	Sagaranten
3202081	3202	Cidadap
3202082	3202	Curugkembar
3202090	3202	Pabuaran
3202100	3202	Lengkong
3202110	3202	Palabuhanratu
3202111	3202	Simpenan
3202120	3202	Warung Kiara
3202121	3202	Bantargadung
3202130	3202	timepang Tengah
3202131	3202	Purabaya
3202140	3202	Cikembar
3202150	3202	Nyalindung
3202160	3202	Geger Bitung
3202170	3202	Sukaraja
3202171	3202	Kebonpedes
3202172	3202	Cireunghas
3202173	3202	Sukalarang
3202180	3202	Sukabumi
3202190	3202	Kadudampit
3202200	3202	Cisaat
3202201	3202	Gunungguruh
3202210	3202	Cibadak
3202211	3202	Cicantayan
3202212	3202	Caringin
3202220	3202	Nagrak
3202221	3202	Ciambar
3202230	3202	Cicurug
3202240	3202	Cidahu
3202250	3202	Parakan Salak
3202260	3202	Parung Kuda
3202261	3202	Bojong Genteng
3202270	3202	Kalapa Nunggal
3202280	3202	Cikidang
3202290	3202	Cisolok
3202291	3202	Cikakak
3202300	3202	Kabandungan
3203010	3203	Agrabinta
3203011	3203	Leles
3203020	3203	Sindangbarang
3203030	3203	Cidaun
3203040	3203	Naringgul
3203050	3203	Cibinong
3203051	3203	Cikadu
3203060	3203	Tanggeung
3203061	3203	Pasirkuda
3203070	3203	Kadupandak
3203071	3203	Cijati
3203080	3203	Takokak
3203090	3203	Sukanagara
3203100	3203	Pagelaran
3203110	3203	Campaka
3203111	3203	Campaka Mulya
3203120	3203	Cibeber
3203130	3203	Warungkondang
3203131	3203	Gekbrong
3203140	3203	Cilaku
3203150	3203	Sukaluyu
3203160	3203	Bojongpicung
3203161	3203	Haurwangi
3203170	3203	Ciranjang
3203180	3203	Mande
3203190	3203	Karangtengah
3203200	3203	Cianjur
3203210	3203	Cugenang
3203220	3203	Pacet
3203221	3203	Cipanas
3203230	3203	Sukaresmi
3203240	3203	Cikalongkulon
3204010	3204	Ciwidey
3204011	3204	Rancabali
3204020	3204	Pasirtimebu
3204030	3204	Cimaung
3204040	3204	Pangalengan
3204050	3204	Kertasari
3204060	3204	Pacet
3204070	3204	Ibun
3204080	3204	Paseh
3204090	3204	Cikancung
3204100	3204	Cicalengka
3204101	3204	Nagreg
3204110	3204	Rancaekek
3204120	3204	Majalaya
3204121	3204	Solokan Jeruk
3204130	3204	Ciparay
3204140	3204	Baleendah
3204150	3204	Arjasari
3204160	3204	Banjaran
3204161	3204	Cangkuang
3204170	3204	Pameungpeuk
3204180	3204	Katapang
3204190	3204	Soreang
3204191	3204	Kutawaringin
3204250	3204	Margaasih
3204260	3204	Margahayu
3204270	3204	Dayeuhkolot
3204280	3204	Bojongsoang
3204290	3204	Cileunyi
3204300	3204	Cilengkrang
3204310	3204	Cimenyan
3205010	3205	Cisewu
3205011	3205	Caringin
3205020	3205	Talegong
3205030	3205	Bungmonthg
3205031	3205	Mekarmukti
3205040	3205	Pamulihan
3205050	3205	Pakenjeng
3205060	3205	Cikelet
3205070	3205	Pameungpeuk
3205080	3205	Cibalong
3205090	3205	Cisompet
3205100	3205	Peundeuy
3205110	3205	Singajaya
3205111	3205	Cihurip
3205120	3205	Cikajang
3205130	3205	Banjarwangi
3205140	3205	Cilawu
3205150	3205	Bayongbong
3205151	3205	Cigedug
3205160	3205	Cisurupan
3205161	3205	Sukaresmi
3205170	3205	Samarang
3205171	3205	Pasirwangi
3205181	3205	Tarogong Kidul
3205182	3205	Tarogong Kaler
3205190	3205	Garut city
3205200	3205	Karangpawitan
3205210	3205	Wanaraja
3205211	3205	Sucinaraja
3205212	3205	Pangatikan
3205220	3205	Sukawening
3205221	3205	Karangtengah
3205230	3205	Banyuresmi
3205240	3205	Leles
3205250	3205	Leuwigoong
3205260	3205	Cibatu
3205261	3205	Kersamanah
3205270	3205	Cibiuk
3205280	3205	Kadungora
3205290	3205	Blubur Limbangan
3205300	3205	Selaawi
3205310	3205	Malangbong
3206010	3206	Cipatujah
3206020	3206	Karangnunggal
3206030	3206	Cikalong
3206040	3206	Pancatengah
3206050	3206	Cikatomas
3206060	3206	Cibalong
3206061	3206	Parungponteng
3206070	3206	Bantarkalong
3206071	3206	Bojongasih
3206072	3206	Culamega
3206080	3206	Bojonggambir
3206090	3206	Sodonghilir
3206100	3206	Taraju
3206110	3206	Salawu
3206111	3206	Puspahiang
3206120	3206	Tanjungjaya
3206130	3206	Sukaraja
3206140	3206	Salopa
3206141	3206	Jatiwaras
3206150	3206	Cineam
3206151	3206	Karangjaya
3206160	3206	Manonjaya
3206161	3206	Gunungtanjung
3206190	3206	Singaparna
3206191	3206	Sukarame
3206192	3206	Mangunreja
3206200	3206	Cigalontang
3206210	3206	Leuwisari
3206211	3206	Sariwangi
3206212	3206	Padakembang
3206221	3206	Sukaratu
3206230	3206	Cisayong
3206231	3206	Sukahening
3206240	3206	Rajapolah
3206250	3206	timeanis
3206260	3206	Ciawi
3206261	3206	Kadipaten
3206270	3206	Pagerageung
3206271	3206	Sukaresik
3207100	3207	Banjarsari
3207101	3207	Banjaranyar
3207110	3207	Lakbok
3207111	3207	Purwadadi
3207120	3207	Pamarican
3207130	3207	Cidolog
3207140	3207	Cimaragas
3207150	3207	Cijeungjing
3207160	3207	Cisaga
3207170	3207	Tambaksari
3207180	3207	Rancah
3207190	3207	Rajacountry
3207200	3207	Sukadana
3207210	3207	Ciamis
3207211	3207	Baregbeg
3207220	3207	Cikoneng
3207221	3207	Sindangkasih
3207230	3207	Cihaurbeuti
3207240	3207	Sadananya
3207250	3207	Cipaku
3207260	3207	Jatinagara
3207270	3207	Panawangan
3207280	3207	Kawali
3207281	3207	Lumbung
3207290	3207	Panjalu
3207291	3207	Sukamantri
3207300	3207	Panumbangan
3208010	3208	Darma
3208020	3208	Kadugede
3208021	3208	Nusaherang
3208030	3208	Ciniru
3208031	3208	Hantara
3208040	3208	Selatimebe
3208050	3208	Subang
3208051	3208	Cilebak
3208060	3208	Ciwaru
3208061	3208	Karangkancana
3208070	3208	Cibingbin
3208071	3208	Cibeureum
3208080	3208	Luragung
3208081	3208	Cimahi
3208090	3208	Cidahu
3208091	3208	Kalimanggis
3208100	3208	Ciawigebang
3208101	3208	Cipicung
3208110	3208	Lebakwangi
3208111	3208	Maleber
3208120	3208	Garawangi
3208121	3208	Sindangagung
3208130	3208	Kuningan
3208140	3208	Cigugur
3208150	3208	Kramatmulya
3208160	3208	Jalaksana
3208161	3208	Japara
3208170	3208	Cilimus
3208171	3208	Cigandamekar
3208180	3208	Mandirancan
3208181	3208	Pancalang
3208190	3208	Pasawahan
3209010	3209	Waled
3209011	3209	Pasaleman
3209020	3209	Ciledug
3209021	3209	Pabuaran
3209030	3209	Losari
3209031	3209	Pabedilan
3209040	3209	Babakan
3209041	3209	Gebang
3209050	3209	Karangsembung
3209051	3209	Karangwareng
3209060	3209	Lemahabang
3209061	3209	Susukanlebak
3209070	3209	Sedong
3209080	3209	Astanajapura
3209081	3209	Pangenan
3209090	3209	Mundu
3209100	3209	Beber
3209101	3209	Greged
3209111	3209	Talun
3209120	3209	Sumber
3209121	3209	Dukupuntang
3209130	3209	Palimanan
3209140	3209	Plumbon
3209141	3209	Depok
3209150	3209	Weru
3209151	3209	Plered
3209161	3209	Tengah Tani
3209162	3209	Kedawung
3209171	3209	Gunungjati
3209180	3209	Kapetakan
3209181	3209	Suranenggala
3209190	3209	Klangenan
3209191	3209	timeblang
3209200	3209	Arjawinangun
3209201	3209	Panguragan
3209210	3209	Ciwaringin
3209211	3209	Gempol
3209220	3209	Susukan
3209230	3209	Gegesik
3209231	3209	Kaliwedi
3210010	3210	Lemahsugih
3210020	3210	Bantarujeg
3210021	3210	Malausma
3210030	3210	Cikijing
3210031	3210	Cingambul
3210040	3210	Talaga
3210041	3210	Banjaran
3210050	3210	Argapura
3210060	3210	Maja
3210070	3210	Majalengka
3210080	3210	Cigasong
3210090	3210	Sukahaji
3210091	3210	Sindang
3210100	3210	Rajagaluh
3210110	3210	Sindangwangi
3210120	3210	Leuwimunding
3210130	3210	Palasah
3210140	3210	Jatiwangi
3210150	3210	Dawuan
3210151	3210	Kasokandel
3210160	3210	Panyingkiran
3210170	3210	Kadipaten
3210180	3210	Kertajati
3210190	3210	Jatitujuh
3210200	3210	Ligung
3210210	3210	Sumberjaya
3211010	3211	Jatinangor
3211020	3211	Cimanggung
3211030	3211	Tanjungsari
3211031	3211	Sukasari
3211032	3211	Pamulihan
3211040	3211	Rancakalong
3211050	3211	Sumedang Selatan
3211060	3211	Sumedang Utara
3211061	3211	Ganeas
3211070	3211	Situraja
3211071	3211	Cisitu
3211080	3211	Darmaraja
3211090	3211	Cibugel
3211100	3211	Wado
3211101	3211	Jatinunggal
3211111	3211	Jatigede
3211120	3211	Tomo
3211130	3211	Ujung Jaya
3211140	3211	Conggeang
3211150	3211	Paseh
3211160	3211	Cimalaka
3211161	3211	Cisarua
3211170	3211	Tanjungkerta
3211171	3211	Tanjungmedar
3211180	3211	Buahdua
3211181	3211	Surian
3212010	3212	Haurgeulis
3212011	3212	Gantar
3212020	3212	Kroya
3212030	3212	Gabuswetan
3212040	3212	Cikedung
3212041	3212	Terisi
3212050	3212	Lelea
3212060	3212	Bangodua
3212061	3212	Tukdana
3212070	3212	Widasari
3212080	3212	Kertasemaya
3212081	3212	Sukagumiwang
3212090	3212	Krangkeng
3212100	3212	Karangampel
3212101	3212	Kedokan Bunder
3212110	3212	Juntinyuat
3212120	3212	Sliyeg
3212130	3212	Jatibarang
3212140	3212	Balongan
3212150	3212	Indramayu
3212160	3212	Sindang
3212161	3212	Cantigi
3212162	3212	Pasekan
3212170	3212	Lohbener
3212171	3212	Arahan
3212180	3212	Losarang
3212190	3212	Kandanghaur
3212200	3212	Bongas
3212210	3212	Anjatan
3212220	3212	Sukra
3212221	3212	Patrol
3213010	3213	Sagalaherang
3213011	3213	Serangpanjang
3213020	3213	Jalancagak
3213021	3213	Ciater
3213030	3213	Cisalak
3213031	3213	Kasomalang
3213040	3213	Tanjungsiang
3213050	3213	Citimebe
3213060	3213	Cibogo
3213070	3213	Subang
3213080	3213	Kalijati
3213081	3213	Dawuan
3213090	3213	Cipeundeuy
3213100	3213	Pabuaran
3213110	3213	Patokbeusi
3213120	3213	Purwadadi
3213130	3213	Cikaum
3213140	3213	Pagaden
3213141	3213	Pagaden Barat
3213150	3213	Cipunagara
3213160	3213	Compreng
3213170	3213	Binong
3213171	3213	Tambakdahan
3213180	3213	Ciasem
3213190	3213	Pamanukan
3213191	3213	Sukasari
3213200	3213	Pusakanagara
3213201	3213	Pusakajaya
3213210	3213	Legonkulon
3213220	3213	Blanakan
3214010	3214	Jatiluhur
3214011	3214	Sukasari
3214020	3214	Maniis
3214030	3214	Tegal Waru
3214040	3214	Plered
3214050	3214	Sukatani
3214060	3214	Darangdan
3214070	3214	Bojong
3214080	3214	Wanayasa
3214081	3214	Kiarapedes
3214090	3214	Pasawahan
3214091	3214	Pondok Salam
3214100	3214	Purwakarta
3214101	3214	Babakancikao
3214110	3214	Campaka
3214111	3214	Cibatu
3214112	3214	Bungursari
3215010	3215	Pangkalan
3215011	3215	Tegalwaru
3215020	3215	Ciampel
3215031	3215	Teluktimebe Timur
3215032	3215	Teluktimebe Barat
3215040	3215	Klari
3215050	3215	Cikampek
3215051	3215	Purwasari
3215060	3215	Tirtamulya
3215070	3215	Jatisari
3215071	3215	Banyusari
3215072	3215	citybaru
3215081	3215	Cilamaya Wetan
3215082	3215	Cilamaya Kulon
3215090	3215	Lemahabang
3215100	3215	Talagasari
3215111	3215	Majalaya
3215112	3215	Karawang Timur
3215113	3215	Karawang Barat
3215120	3215	Rawamerta
3215130	3215	Tempuran
3215140	3215	Kutawaluya
3215150	3215	Rengasdengklok
3215151	3215	Jayakerta
3215160	3215	Pedes
3215161	3215	Cilebar
3215170	3215	Cibuaya
3215180	3215	Tirtajaya
3215190	3215	Batujaya
3215200	3215	Pakisjaya
3216010	3216	Setu
3216021	3216	Serang Baru
3216022	3216	Cikarang Pusat
3216023	3216	Cikarang Selatan
3216030	3216	Cibarusah
3216031	3216	Bojongmangu
3216041	3216	Cikarang Timur
3216050	3216	Kedungwaringin
3216061	3216	Cikarang Utara
3216062	3216	Karangbahagia
3216070	3216	Cibitung
3216071	3216	Cikarang Barat
3216081	3216	Tambun Selatan
3216082	3216	Tambun Utara
3216090	3216	Babelan
3216100	3216	Tarumajaya
3216110	3216	Tambelang
3216111	3216	Sukawangi
3216120	3216	Sukatani
3216121	3216	Sukakarya
3216130	3216	Pebayuran
3216140	3216	Cabangbungin
3216150	3216	Muara Gembong
3217010	3217	Rongga
3217020	3217	Gununghalu
3217030	3217	Sindangkerta
3217040	3217	Cililin
3217050	3217	Cihampelas
3217060	3217	Cipongkor
3217070	3217	Batujajar
3217071	3217	Saguling
3217080	3217	Cipatat
3217090	3217	Padalarang
3217100	3217	Ngamprah
3217110	3217	Parongpong
3217120	3217	Lembang
3217130	3217	Cisarua
3217140	3217	Cikalong Wetan
3217150	3217	Cipeundeuy
3218010	3218	Cimerak
3218020	3218	Cijulang
3218030	3218	Cigugur
3218040	3218	Langkaplancar
3218050	3218	Parigi
3218060	3218	Sidamulih
3218070	3218	Pangandaran
3218080	3218	Kalipucang
3218090	3218	Padaherang
3218100	3218	Mangunjaya
3271010	3271	Bogor Selatan
3271020	3271	Bogor Timur
3271030	3271	Bogor Utara
3271040	3271	Bogor Tengah
3271050	3271	Bogor Barat
3271060	3271	Tanah Sereal
3272010	3272	Baros
3272011	3272	overtimesitu
3272012	3272	Cibeureum
3272020	3272	Citamiang
3272030	3272	Warudoyong
3272040	3272	Gunung Puyuh
3272050	3272	Cikole
3273010	3273	Bandung Kulon
3273020	3273	Babakan Ciparay
3273030	3273	Bojongloa Kaler
3273040	3273	Bojongloa Kidul
3273050	3273	Astanaanyar
3273060	3273	Regol
3273070	3273	Lengkong
3273080	3273	Bandung Kidul
3273090	3273	Buahbatu
3273100	3273	Rancasari
3273101	3273	Gedebage
3273110	3273	Cibiru
3273111	3273	Panyileukan
3273120	3273	Ujung Berung
3273121	3273	Cinambo
3273130	3273	Arcamanik
3273141	3273	Antapani
3273142	3273	Mandalajati
3273150	3273	Kiaracondong
3273160	3273	Batununggal
3273170	3273	Sumur Bandung
3273180	3273	Andir
3273190	3273	Cicendo
3273200	3273	Bandung Wetan
3273210	3273	Cibeunying Kidul
3273220	3273	Cibeunying Kaler
3273230	3273	Coblong
3273240	3273	Sukajadi
3273250	3273	Sukasari
3273260	3273	Cidadap
3274010	3274	Hartimeukti
3274020	3274	Lemahwungkuk
3274030	3274	Pekalipan
3274040	3274	Kesambi
3274050	3274	Kejaksan
3275010	3275	Pondokgede
3275011	3275	Jatisampurna
3275012	3275	Pondokmelati
3275020	3275	Jatiasih
3275030	3275	Bantargebang
3275031	3275	Mustikajaya
3275040	3275	Bekasi Timur
3275041	3275	Rawalumbu
3275050	3275	Bekasi Selatan
3275060	3275	Bekasi Barat
3275061	3275	Medan Satria
3275070	3275	Bekasi Utara
3276010	3276	Sawangan
3276011	3276	Bojongsari
3276020	3276	Pancoran Mas
3276021	3276	Cipayung
3276030	3276	Sukma Jaya
3276031	3276	Cilodong
3276040	3276	Cimanggis
3276041	3276	Tapos
3276050	3276	Beji
3276060	3276	Limo
3276061	3276	Cinere
3277010	3277	Cimahi Selatan
3277020	3277	Cimahi Tengah
3277030	3277	Cimahi Utara
3278010	3278	Kawalu
3278020	3278	Tamansari
3278030	3278	Cibeureum
3278031	3278	Purbaratu
3278040	3278	Tawang
3278050	3278	Cihideung
3278060	3278	Mangkubumi
3278070	3278	Indihiang
3278071	3278	Bungursari
3278080	3278	Cipedes
3279010	3279	Banjar
3279020	3279	Purwaharja
3279030	3279	Pataruman
3279040	3279	Langensari
3301010	3301	Dayeuhluhur
3301020	3301	Wanareja
3301030	3301	Majenang
3301040	3301	Cimanggu
3301050	3301	Karangpucung
3301060	3301	Cipari
3301070	3301	Sidareja
3301080	3301	Kedungreja
3301090	3301	Patimuan
3301100	3301	Gandrungmangu
3301110	3301	Bantarsari
3301120	3301	Kawunganten
3301121	3301	Kampung Laut
3301130	3301	Jeruklegi
3301140	3301	Kesugihan
3301150	3301	Adipala
3301160	3301	Maos
3301170	3301	Sampang
3301180	3301	Kroya
3301190	3301	Binangun
3301200	3301	Nusawungu
3301710	3301	Cilacap Selatan
3301720	3301	Cilacap Tengah
3301730	3301	Cilacap Utara
3302010	3302	Lumbir
3302020	3302	Wangon
3302030	3302	Jatilawang
3302040	3302	Rawalo
3302050	3302	Kebasen
3302060	3302	Kemranjen
3302070	3302	Sumpiuh
3302080	3302	Tambak
3302090	3302	Somagede
3302100	3302	Kalibagor
3302110	3302	Banyumas
3302120	3302	Patikraja
3302130	3302	Purwojati
3302140	3302	Ajibarang
3302150	3302	Gumelar
3302160	3302	Pekuncen
3302170	3302	Cilongok
3302180	3302	Karanglewas
3302190	3302	Kedung Banteng
3302200	3302	Baturraden
3302210	3302	Sumbang
3302220	3302	Kembaran
3302230	3302	Sokaraja
3302710	3302	Purwokerto Selatan
3302720	3302	Purwokerto Barat
3302730	3302	Purwokerto Timur
3302740	3302	Purwokerto Utara
3303010	3303	Kemangkon
3303020	3303	Bukateja
3303030	3303	Kejobong
3303040	3303	Pengadegan
3303050	3303	Kaligondang
3303060	3303	Purbalingga
3303070	3303	Kalimanah
3303080	3303	Padamara
3303090	3303	Kutasari
3303100	3303	Bojongsari
3303110	3303	Mrebet
3303120	3303	Bobotsari
3303130	3303	Karangreja
3303131	3303	Karangtimebu
3303140	3303	Karanganyar
3303141	3303	Kertanegara
3303150	3303	Karangmoncol
3303160	3303	Rembang
3304010	3304	Susukan
3304020	3304	Purwareja Klampok
3304030	3304	Mandiraja
3304040	3304	Purwanegara
3304050	3304	Bawang
3304060	3304	Banjarnegara
3304061	3304	Pagedongan
3304070	3304	Sigaluh
3304080	3304	Madukara
3304090	3304	Banjarmangu
3304100	3304	Wanadadi
3304110	3304	Rakit
3304120	3304	Punggelan
3304130	3304	Karangkobar
3304140	3304	Pagentan
3304150	3304	Pejawaran
3304160	3304	Batur
3304170	3304	Wanayasa
3304180	3304	Kalibening
3304181	3304	Pandanarum
3305010	3305	Ayah
3305020	3305	Buayan
3305030	3305	Puring
3305040	3305	Petanahan
3305050	3305	Klirong
3305060	3305	Buluspesantren
3305070	3305	Ambal
3305080	3305	Mirit
3305081	3305	Bonorowo
3305090	3305	Prembun
3305091	3305	Padureso
3305100	3305	Kutowinangun
3305110	3305	Alian
3305111	3305	Poncowarno
3305120	3305	Kebumen
3305130	3305	Pejagoan
3305140	3305	Sruweng
3305150	3305	Adimulyo
3305160	3305	Kuwarasan
3305170	3305	Rowokele
3305180	3305	Sempor
3305190	3305	Gombong
3305200	3305	Karanganyar
3305210	3305	Karanggayam
3305220	3305	Sadang
3305221	3305	Karangsambung
3306010	3306	Grabag
3306020	3306	Ngombol
3306030	3306	Purwodadi
3306040	3306	Bagelen
3306050	3306	Kaligesing
3306060	3306	Purworejo
3306070	3306	Banyu Urip
3306080	3306	Bayan
3306090	3306	Kutoarjo
3306100	3306	Butuh
3306110	3306	Pituruh
3306120	3306	Kemiri
3306130	3306	Bruno
3306140	3306	Gebang
3306150	3306	Loano
3306160	3306	Bener
3307010	3307	Wadaslintang
3307020	3307	Kepil
3307030	3307	Sapuran
3307031	3307	Kalibawang
3307040	3307	Kaliwiro
3307050	3307	Leksono
3307051	3307	Sukoharjo
3307060	3307	Selomerto
3307070	3307	Kalikajar
3307080	3307	Kertek
3307090	3307	Wonosobo
3307100	3307	Watumalang
3307110	3307	Mojotengah
3307120	3307	Garung
3307130	3307	Kejajar
3308010	3308	Salaman
3308020	3308	Borobudur
3308030	3308	Ngluwar
3308040	3308	Salam
3308050	3308	Srumbung
3308060	3308	Dukun
3308070	3308	Muntilan
3308080	3308	Mungkid
3308090	3308	Sawangan
3308100	3308	Candimulyo
3308110	3308	Mertoyudan
3308120	3308	Tempuran
3308130	3308	Kajoran
3308140	3308	Kaliangkrik
3308150	3308	Bandongan
3308160	3308	Windusari
3308170	3308	Secang
3308180	3308	Tegalrejo
3308190	3308	Pakis
3308200	3308	Grabag
3308210	3308	Ngablak
3309010	3309	Selo
3309020	3309	Ampel
3309030	3309	Cepogo
3309040	3309	Musuk
3309050	3309	Boyolali
3309060	3309	Mojosongo
3309070	3309	Teras
3309080	3309	Sawit
3309090	3309	Banyudono
3309100	3309	Sambi
3309110	3309	Ngemplak
3309120	3309	Nogosari
3309130	3309	Simo
3309140	3309	Karanggede
3309150	3309	Klego
3309160	3309	Andong
3309170	3309	Kemusu
3309180	3309	Wonosegoro
3309190	3309	Juwangi
3310010	3310	Prambanan
3310020	3310	Gantiwarno
3310030	3310	Wedi
3310040	3310	Bayat
3310050	3310	Cawas
3310060	3310	Trucuk
3310070	3310	Kalikotes
3310080	3310	Kebonarum
3310090	3310	Jogonalan
3310100	3310	Manisrenggo
3310110	3310	Karangnongko
3310120	3310	Ngawen
3310130	3310	Ceper
3310140	3310	Pedan
3310150	3310	Karangdowo
3310160	3310	Juwiring
3310170	3310	Wonosari
3310180	3310	Delanggu
3310190	3310	Polanharjo
3310200	3310	Karanganom
3310210	3310	Tulung
3310220	3310	Jatinom
3310230	3310	Kemalang
3310710	3310	Klaten Selatan
3310720	3310	Klaten Tengah
3310730	3310	Klaten Utara
3311010	3311	Weru
3311020	3311	Bulu
3311030	3311	Tawangsari
3311040	3311	Sukoharjo
3311050	3311	Nguter
3311060	3311	Bendosari
3311070	3311	Polokarto
3311080	3311	Mojolaban
3311090	3311	Grogol
3311100	3311	Baki
3311110	3311	Gatak
3311120	3311	Kartasura
3312010	3312	Pracimantoro
3312020	3312	Paranggupito
3312030	3312	Giritontro
3312040	3312	Giriwoyo
3312050	3312	Batuwarno
3312060	3312	Karangtengah
3312070	3312	Tirtomoyo
3312080	3312	Nguntoronadi
3312090	3312	Baturetno
3312100	3312	Eromoko
3312110	3312	Wuryantoro
3312120	3312	Manyaran
3312130	3312	Selogiri
3312140	3312	Wonogiri
3312150	3312	Ngadirojo
3312160	3312	Sidoharjo
3312170	3312	Jatiroto
3312180	3312	Kismantoro
3312190	3312	Purwantoro
3312200	3312	Bulukerto
3312201	3312	Puhpelem
3312210	3312	Slogohimo
3312220	3312	Jatisrono
3312230	3312	Jatipurno
3312240	3312	Girimarto
3313010	3313	Jatipuro
3313020	3313	Jatiyoso
3313030	3313	Jumapolo
3313040	3313	Jumantono
3313050	3313	Matesih
3313060	3313	Tawangmangu
3313070	3313	Ngargoyoso
3313080	3313	Karangpandan
3313090	3313	Karanganyar
3313100	3313	Tasikmadu
3313110	3313	Jaten
3313120	3313	Colomadu
3313130	3313	Gondangrejo
3313140	3313	Kebakkramat
3313150	3313	Mojogedang
3313160	3313	Kerjo
3313170	3313	Jenawi
3314010	3314	Kalitimebe
3314020	3314	Plupuh
3314030	3314	Masaran
3314040	3314	Kedawung
3314050	3314	Sambirejo
3314060	3314	Gondang
3314070	3314	Sambung Macan
3314080	3314	Ngrampal
3314090	3314	Karangmalang
3314100	3314	Sragen
3314110	3314	Sidoharjo
3314120	3314	Tanon
3314130	3314	Gemolong
3314140	3314	Miri
3314150	3314	Sumberlawang
3314160	3314	Mondokan
3314170	3314	Sukodono
3314180	3314	Gesi
3314190	3314	Tangen
3314200	3314	Jenar
3315010	3315	Kedungjati
3315020	3315	Karangrayung
3315030	3315	Penawangan
3315040	3315	Toroh
3315050	3315	Geyer
3315060	3315	Pulokulon
3315070	3315	Kradenan
3315080	3315	Gabus
3315090	3315	Ngaringan
3315100	3315	Wirosari
3315110	3315	Tawangharjo
3315120	3315	Grobogan
3315130	3315	Purwodadi
3315140	3315	Brati
3315150	3315	Klambu
3315160	3315	Godong
3315170	3315	Gubug
3315180	3315	Tegowanu
3315190	3315	Tanggungharjo
3316010	3316	Jati
3316020	3316	Randublatung
3316030	3316	Kradenan
3316040	3316	Kedungtuban
3316050	3316	Cepu
3316060	3316	Sambong
3316070	3316	Jiken
3316080	3316	Bogorejo
3316090	3316	Jepon
3316100	3316	city Blora
3316110	3316	Banjarejo
3316120	3316	Tunjungan
3316130	3316	Japah
3316140	3316	Ngawen
3316150	3316	Kunduran
3316160	3316	Todanan
3317010	3317	Sumber
3317020	3317	Bulu
3317030	3317	Gunem
3317040	3317	Sale
3317050	3317	Sarang
3317060	3317	Sedan
3317070	3317	Pamotan
3317080	3317	Sulang
3317090	3317	Kaliori
3317100	3317	Rembang
3317110	3317	Pancur
3317120	3317	Kragan
3317130	3317	Sluke
3317140	3317	Lasem
3318010	3318	Sukolilo
3318020	3318	Kayen
3318030	3318	Tambakromo
3318040	3318	Winong
3318050	3318	Pucakwangi
3318060	3318	Jaken
3318070	3318	Batangan
3318080	3318	Juwana
3318090	3318	Jakenan
3318100	3318	Pati
3318110	3318	Gabus
3318120	3318	Margorejo
3318130	3318	Gembong
3318140	3318	Tlogowungu
3318150	3318	Wedarijaksa
3318160	3318	Trangkil
3318170	3318	Margoyoso
3318180	3318	Gunung Wungkal
3318190	3318	Cluwak
3318200	3318	Tayu
3318210	3318	Dukuhseti
3319010	3319	Kaliwungu
3319020	3319	city Kudus
3319030	3319	Jati
3319040	3319	Undaan
3319050	3319	Mejobo
3319060	3319	Jekulo
3319070	3319	Bae
3319080	3319	Gebog
3319090	3319	Dawe
3320010	3320	Kedung
3320020	3320	Pecangaan
3320021	3320	Kalinyamatan
3320030	3320	Welahan
3320040	3320	Mayong
3320050	3320	Nalumsari
3320060	3320	Batealit
3320070	3320	yearan
3320080	3320	Jepara
3320090	3320	Mlonggo
3320091	3320	Pakis Aji
3320100	3320	Bangsri
3320101	3320	Kembang
3320110	3320	Keling
3320111	3320	Donorojo
3320120	3320	Karimunjawa
3321010	3321	Mranggen
3321020	3321	Karangawen
3321030	3321	Guntur
3321040	3321	Sayung
3321050	3321	Karang Tengah
3321060	3321	Bonang
3321070	3321	Demak
3321080	3321	Wonosalam
3321090	3321	Dempet
3321091	3321	Kebonagung
3321100	3321	Gajah
3321110	3321	Karanganyar
3321120	3321	Mijen
3321130	3321	Wedung
3322010	3322	Getasan
3322020	3322	Tengaran
3322030	3322	Susukan
3322031	3322	Kaliwungu
3322040	3322	Suruh
3322050	3322	Pabelan
3322060	3322	Tuntang
3322070	3322	Banyubiru
3322080	3322	timebu
3322090	3322	Sumowono
3322100	3322	Ambarawa
3322101	3322	Bandungan
3322110	3322	Bawen
3322120	3322	Bringin
3322121	3322	Bancak
3322130	3322	Pringapus
3322140	3322	Bergas
3322151	3322	Ungaran Barat
3322152	3322	Ungaran Timur
3323010	3323	Parakan
3323011	3323	Kledung
3323012	3323	Bansari
3323020	3323	Bulu
3323030	3323	Temanggung
3323031	3323	Tlogomulyo
3323040	3323	Tembarak
3323041	3323	Selopampang
3323050	3323	Kranggan
3323060	3323	Pringsurat
3323070	3323	Kaloran
3323080	3323	Kandangan
3323090	3323	Kedu
3323100	3323	Ngadirejo
3323110	3323	Jumo
3323111	3323	Gemawang
3323120	3323	Candiroto
3323121	3323	Bejen
3323130	3323	Tretep
3323131	3323	Wonoboyo
3324010	3324	Plantungan
3324020	3324	Sukorejo
3324030	3324	Pagerruyung
3324040	3324	Patean
3324050	3324	Singorojo
3324060	3324	Limbangan
3324070	3324	Boja
3324080	3324	Kaliwungu
3324081	3324	Kaliwungu Selatan
3324090	3324	Brangsong
3324100	3324	Pegandon
3324101	3324	Ngampel
3324110	3324	Gemuh
3324111	3324	Ringinarum
3324120	3324	Weleri
3324130	3324	Rowosari
3324140	3324	Kangkung
3324150	3324	Cepiring
3324160	3324	Patebon
3324170	3324	city Kendal
3325010	3325	Wonotunggal
3325020	3325	Bandar
3325030	3325	Blado
3325040	3325	Reban
3325050	3325	Bawang
3325060	3325	Tersono
3325070	3325	Gringsing
3325080	3325	Limpung
3325081	3325	Banyuputih
3325090	3325	Subah
3325091	3325	Pecalungan
3325100	3325	Tulis
3325101	3325	Kandeman
3325110	3325	Batang
3325120	3325	Warung Asem
3326010	3326	Kandangserang
3326020	3326	Paninggaran
3326030	3326	Lebakbarang
3326040	3326	Petungkriono
3326050	3326	Talun
3326060	3326	Doro
3326070	3326	Karanganyar
3326080	3326	Kajen
3326090	3326	Kesesi
3326100	3326	Sragi
3326101	3326	Siwalan
3326110	3326	Bojong
3326120	3326	Wonopringgo
3326130	3326	Kedungwuni
3326131	3326	Karangdadap
3326140	3326	Buaran
3326150	3326	Tirto
3326160	3326	Wiracountry
3326161	3326	Wonokerto
3327010	3327	Moga
3327011	3327	Warungpring
3327020	3327	Pulosari
3327030	3327	Belik
3327040	3327	Watukumpul
3327050	3327	Bodeh
3327060	3327	Bantarbolang
3327070	3327	Randudongkal
3327080	3327	Pemalang
3327090	3327	Taman
3327100	3327	Petarukan
3327110	3327	Ampelgading
3327120	3327	Comal
3327130	3327	Ulutimei
3328010	3328	Margasari
3328020	3328	Bumijawa
3328030	3328	Bojong
3328040	3328	Balapulang
3328050	3328	Pagerbarang
3328060	3328	Lebaksiu
3328070	3328	Jatinegara
3328080	3328	Kedung Banteng
3328090	3328	Pangkah
3328100	3328	Slawi
3328110	3328	Dukuhwaru
3328120	3328	Adiwerna
3328130	3328	Dukuhturi
3328140	3328	Talang
3328150	3328	Tarub
3328160	3328	Kramat
3328170	3328	Suradadi
3328180	3328	Warureja
3329010	3329	Salem
3329020	3329	Bantarkawung
3329030	3329	Bumiayu
3329040	3329	Paguyangan
3329050	3329	Sirampog
3329060	3329	Tonjong
3329070	3329	Larangan
3329080	3329	Ketanggungan
3329090	3329	Banjarharjo
3329100	3329	Losari
3329110	3329	Tanjung
3329120	3329	Kersana
3329130	3329	Bulakamba
3329140	3329	Wanasari
3329150	3329	Songgom
3329160	3329	Jatibarang
3329170	3329	Brebes
3371010	3371	Magelang Selatan
3371011	3371	Magelang Tengah
3371020	3371	Magelang Utara
3372010	3372	Laweyan
3372020	3372	Serengan
3372030	3372	Pasar Kliwon
3372040	3372	Jebres
3372050	3372	Banjarsari
3373010	3373	Argomulyo
3373020	3373	Tingkir
3373030	3373	Sidomukti
3373040	3373	Sidorejo
3374010	3374	Mijen
3374020	3374	Gunung Pati
3374030	3374	Banyumanik
3374040	3374	Gajah Mungkur
3374050	3374	Semarang Selatan
3374060	3374	Candisari
3374070	3374	Tembalang
3374080	3374	Pedurungan
3374090	3374	Genuk
3374100	3374	Gayamsari
3374110	3374	Semarang Timur
3374120	3374	Semarang Utara
3374130	3374	Semarang Tengah
3374140	3374	Semarang Barat
3374150	3374	Tugu
3374160	3374	Ngaliyan
3375010	3375	Pekalongan Barat
3375020	3375	Pekalongan Timur
3375030	3375	Pekalongan Selatan
3375040	3375	Pekalongan Utara
3376010	3376	Tegal Selatan
3376020	3376	Tegal Timur
3376030	3376	Tegal Barat
3376040	3376	Margadana
3401010	3401	Temon
3401020	3401	Wates
3401030	3401	Panjatan
3401040	3401	Galur
3401050	3401	Lendah
3401060	3401	Sentolo
3401070	3401	Pengasih
3401080	3401	Kokap
3401090	3401	Girimulyo
3401100	3401	Nanggulan
3401110	3401	Kalibawang
3401120	3401	Samigaluh
3402010	3402	Srandakan
3402020	3402	Sanden
3402030	3402	Kretek
3402040	3402	Pundong
3402050	3402	Bambang Lipuro
3402060	3402	Pandak
3402070	3402	Bantul
3402080	3402	Jetis
3402090	3402	Imogiri
3402100	3402	Dlingo
3402110	3402	Pleret
3402120	3402	Piyungan
3402130	3402	Banguntapan
3402140	3402	Sewon
3402150	3402	Kasihan
3402160	3402	Pajangan
3402170	3402	Sedayu
3403010	3403	Panggang
3403011	3403	Purwosari
3403020	3403	Paliyan
3403030	3403	Sapto Sari
3403040	3403	Tepus
3403041	3403	Tanjungsari
3403050	3403	Rongkop
3403051	3403	Girisubo
3403060	3403	Semanu
3403070	3403	Ponjong
3403080	3403	Karangmojo
3403090	3403	Wonosari
3403100	3403	Playen
3403110	3403	Patuk
3403120	3403	Gedang Sari
3403130	3403	Nglipar
3403140	3403	Ngawen
3403150	3403	Semin
3404010	3404	Moyudan
3404020	3404	Minggir
3404030	3404	Seyegan
3404040	3404	Godean
3404050	3404	Gamping
3404060	3404	Mlati
3404070	3404	Depok
3404080	3404	Berbah
3404090	3404	Prambanan
3404100	3404	Kalasan
3404110	3404	Ngemplak
3404120	3404	Ngaglik
3404130	3404	Sleman
3404140	3404	Tempel
3404150	3404	Turi
3404160	3404	Pakem
3404170	3404	Cangkringan
3471010	3471	Mantrijeron
3471020	3471	Kraton
3471030	3471	Mergangsan
3471040	3471	Umbulharjo
3471050	3471	citygede
3471060	3471	Gondokusuman
3471070	3471	Danurejan
3471080	3471	Pakualaman
3471090	3471	Gondomanan
3471100	3471	Ngampilan
3471110	3471	Wirobrajan
3471120	3471	Gedong Tengen
3471130	3471	Jetis
3471140	3471	Tegalrejo
3501010	3501	Donorojo
3501020	3501	Punung
3501030	3501	Pringkuku
3501040	3501	Pacitan
3501050	3501	Kebonagung
3501060	3501	Arjosari
3501070	3501	Nawangan
3501080	3501	Bandar
3501090	3501	Tegalombo
3501100	3501	Tulakan
3501110	3501	Ngadirojo
3501120	3501	Sudimoro
3502010	3502	Ngrayun
3502020	3502	Slahung
3502030	3502	Bungkal
3502040	3502	Sambit
3502050	3502	Sawoo
3502060	3502	Sooko
3502061	3502	Pudak
3502070	3502	Pulung
3502080	3502	Mlarak
3502090	3502	Siman
3502100	3502	Jetis
3502110	3502	Balong
3502120	3502	Kauman
3502130	3502	timebon
3502140	3502	Badegan
3502150	3502	Sampung
3502160	3502	Sukorejo
3502170	3502	Ponorogo
3502180	3502	Babadan
3502190	3502	Jenangan
3502200	3502	Ngebel
3503010	3503	Panggul
3503020	3503	Munjungan
3503030	3503	Watulimo
3503040	3503	Kampak
3503050	3503	Dongko
3503060	3503	Pule
3503070	3503	Karangan
3503071	3503	Suruh
3503080	3503	Gandusari
3503090	3503	Durenan
3503100	3503	Pogalan
3503110	3503	Trenggalek
3503120	3503	Tugu
3503130	3503	Bendungan
3504010	3504	Besuki
3504020	3504	Bandung
3504030	3504	Pakel
3504040	3504	Campur Darat
3504050	3504	Tanggung Gunung
3504060	3504	Kalidawir
3504070	3504	Pucang Laban
3504080	3504	Rejotangan
3504090	3504	Ngunut
3504100	3504	Sumbergempol
3504110	3504	Boyolangu
3504120	3504	Tulungagung
3504130	3504	Kedungwaru
3504140	3504	Ngantru
3504150	3504	Karangrejo
3504160	3504	Kauman
3504170	3504	Gondang
3504180	3504	Pager Wojo
3504190	3504	Sendang
3505010	3505	Bakung
3505020	3505	Wonotirto
3505030	3505	Panggungrejo
3505040	3505	Wates
3505050	3505	Binangun
3505060	3505	Sutojayan
3505070	3505	Kademangan
3505080	3505	Kanigoro
3505090	3505	Talun
3505100	3505	Selopuro
3505110	3505	Kesamben
3505120	3505	Selorejo
3505130	3505	Doko
3505140	3505	Wlingi
3505150	3505	Gandusari
3505160	3505	Garum
3505170	3505	Nglegok
3505180	3505	Sanankulon
3505190	3505	Ponggok
3505200	3505	Srengat
3505210	3505	Wonodadi
3505220	3505	Udanawu
3506010	3506	Mojo
3506020	3506	Semen
3506030	3506	Ngadiluwih
3506040	3506	Kras
3506050	3506	Ringinrejo
3506060	3506	Kandat
3506070	3506	Wates
3506080	3506	Ngancar
3506090	3506	Plosoklaten
3506100	3506	Gurah
3506110	3506	Puncu
3506120	3506	Kepung
3506130	3506	Kandangan
3506140	3506	Pare
3506141	3506	Badas
3506150	3506	Kunjang
3506160	3506	Plemahan
3506170	3506	Purwoasri
3506180	3506	Papar
3506190	3506	Pagu
3506191	3506	Kayen Kidul
3506200	3506	Gampengrejo
3506201	3506	Ngasem
3506210	3506	Banyakan
3506220	3506	Grogol
3506230	3506	Tarokan
3507010	3507	Donomulyo
3507020	3507	Kalipare
3507030	3507	Pagak
3507040	3507	Bantur
3507050	3507	Gedangan
3507060	3507	Sumbermanjing
3507070	3507	Dampit
3507080	3507	Tirto Yudo
3507090	3507	Ampelgading
3507100	3507	Poncokusumo
3507110	3507	Wajak
3507120	3507	Turen
3507130	3507	Bululawang
3507140	3507	Gondanglegi
3507150	3507	Pagelaran
3507160	3507	Kepanjen
3507170	3507	Sumber Pucung
3507180	3507	Kromengan
3507190	3507	Ngajum
3507200	3507	Wonosari
3507210	3507	Wagir
3507220	3507	Pakisaji
3507230	3507	Tajinan
3507240	3507	Tumpang
3507250	3507	Pakis
3507260	3507	Jabung
3507270	3507	Lawang
3507280	3507	Singosari
3507290	3507	Karangploso
3507300	3507	Dau
3507310	3507	Pujon
3507320	3507	Ngantang
3507330	3507	Kasembon
3508010	3508	Tempursari
3508020	3508	Pronojiwo
3508030	3508	Candipuro
3508040	3508	Pasirian
3508050	3508	Tempeh
3508060	3508	Lumajang
3508061	3508	Sumbersuko
3508070	3508	Tekung
3508080	3508	Kunir
3508090	3508	Yosowilangun
3508100	3508	Rowokangkung
3508110	3508	Jatiroto
3508120	3508	Randuagung
3508130	3508	Sukodono
3508140	3508	Padang
3508150	3508	Pasrutimebe
3508160	3508	Senduro
3508170	3508	Gucialit
3508180	3508	Kedungjajang
3508190	3508	Klakah
3508200	3508	Ranuyoso
3509010	3509	Kencong
3509020	3509	Gumuk Mas
3509030	3509	Puger
3509040	3509	Wuluhan
3509050	3509	Ambulu
3509060	3509	Tempurejo
3509070	3509	Silo
3509080	3509	Mayang
3509090	3509	Mumbulsari
3509100	3509	Jenggawah
3509110	3509	Ajung
3509120	3509	Rambipuji
3509130	3509	Balung
3509140	3509	Umbulsari
3509150	3509	Semboro
3509160	3509	Jombang
3509170	3509	Sumber Baru
3509180	3509	Tanggul
3509190	3509	Bangsalsari
3509200	3509	Panti
3509210	3509	Sukorambi
3509220	3509	Arjasa
3509230	3509	Pakusari
3509240	3509	Kalisat
3509250	3509	Ledokombo
3509260	3509	Sumbertimebe
3509270	3509	Sukowono
3509280	3509	Jelbuk
3509710	3509	Kaliwates
3509720	3509	Sumbersari
3509730	3509	Patrang
3510010	3510	Pesanggaran
3510011	3510	Siliragung
3510020	3510	Bangorejo
3510030	3510	Purwoharjo
3510040	3510	Tegaldlimo
3510050	3510	Muncar
3510060	3510	Cluring
3510070	3510	Gambiran
3510071	3510	Tegalsari
3510080	3510	Glenmore
3510090	3510	Kalibaru
3510100	3510	Genteng
3510110	3510	Srono
3510120	3510	Rogotimepi
3510121	3510	Blimbingsari
3510130	3510	Kabat
3510140	3510	Singojuruh
3510150	3510	Sempu
3510160	3510	Songgon
3510170	3510	Glagah
3510171	3510	Licin
3510180	3510	Banyuwangi
3510190	3510	Giri
3510200	3510	Kalipuro
3510210	3510	Wongsorejo
3511010	3511	Maesan
3511020	3511	Grujugan
3511030	3511	Tamanan
3511031	3511	timebesari Darus Sholah
3511040	3511	Pujer
3511050	3511	Tlogosari
3511060	3511	Sukosari
3511061	3511	Sumber Wringin
3511070	3511	Tapen
3511080	3511	Wonosari
3511090	3511	Tenggarang
3511100	3511	Bondowoso
3511110	3511	Curah Dami
3511111	3511	Binakal
3511120	3511	Pakem
3511130	3511	Wringin
3511140	3511	Tegalampel
3511141	3511	Taman Krocok
3511150	3511	Klabang
3511151	3511	Ijen
3511152	3511	Botolinggo
3511160	3511	Prajekan
3511170	3511	Cermee
3512010	3512	Sumbermalang
3512020	3512	Jatibanteng
3512030	3512	Banyuglugur
3512040	3512	Besuki
3512050	3512	Suboh
3512060	3512	Mlandingan
3512070	3512	Bungatan
3512080	3512	Kendit
3512090	3512	Panarukan
3512100	3512	Situbondo
3512110	3512	Mangaran
3512120	3512	Panji
3512130	3512	Kapongan
3512140	3512	Arjasa
3512150	3512	Jangkar
3512160	3512	Asembagus
3512170	3512	Banyuputih
3513010	3513	Sukapura
3513020	3513	Sumber
3513030	3513	Kuripan
3513040	3513	Bantaran
3513050	3513	Leces
3513060	3513	Tegalsiwalan
3513070	3513	Banyuanyar
3513080	3513	Tiris
3513090	3513	Krucil
3513100	3513	Gading
3513110	3513	Pakuniran
3513120	3513	cityanyar
3513130	3513	Paiton
3513140	3513	Besuk
3513150	3513	Kraksaan
3513160	3513	Krejengan
3513170	3513	Pajarakan
3513180	3513	Maron
3513190	3513	Gending
3513200	3513	Dringu
3513210	3513	Wonomerto
3513220	3513	Lumbang
3513230	3513	Tongas
3513240	3513	Sumberasih
3514010	3514	Purwodadi
3514020	3514	Tutur
3514030	3514	Puspo
3514040	3514	Tosari
3514050	3514	Lumbang
3514060	3514	Pasrepan
3514070	3514	Kejayan
3514080	3514	Wonorejo
3514090	3514	Purwosari
3514100	3514	Prigen
3514110	3514	Sukorejo
3514120	3514	Pandaan
3514130	3514	Gempol
3514140	3514	Beji
3514150	3514	Bangil
3514160	3514	Rembang
3514170	3514	Kraton
3514180	3514	Pohjentrek
3514190	3514	Gondang Wetan
3514200	3514	Rejoso
3514210	3514	Winongan
3514220	3514	Grati
3514230	3514	Lekok
3514240	3514	Nguling
3515010	3515	Tarik
3515020	3515	Prambon
3515030	3515	Krembung
3515040	3515	Porong
3515050	3515	Jabon
3515060	3515	Tanggulangin
3515070	3515	Candi
3515080	3515	Tulangan
3515090	3515	Wonoayu
3515100	3515	Sukodono
3515110	3515	Sidoarjo
3515120	3515	Buduran
3515130	3515	Sedati
3515140	3515	Waru
3515150	3515	Gedangan
3515160	3515	Taman
3515170	3515	Krian
3515180	3515	Balong Bendo
3516010	3516	Jatirejo
3516020	3516	Gondang
3516030	3516	Pacet
3516040	3516	Trawas
3516050	3516	Ngoro
3516060	3516	Pungging
3516070	3516	Kutorejo
3516080	3516	Mojosari
3516090	3516	Bangsal
3516091	3516	Mojoanyar
3516100	3516	Dlanggu
3516110	3516	Puri
3516120	3516	Trowulan
3516130	3516	Sooko
3516140	3516	Gedek
3516150	3516	Kemlagi
3516160	3516	Jetis
3516170	3516	Dawar Blandong
3517010	3517	Bandar Kedung Mulyo
3517020	3517	Perak
3517030	3517	Gudo
3517040	3517	Diwek
3517050	3517	Ngoro
3517060	3517	Mojowarno
3517070	3517	Bareng
3517080	3517	Wonosalam
3517090	3517	Mojoagung
3517100	3517	Sumobito
3517110	3517	Jogo Roto
3517120	3517	Peterongan
3517130	3517	Jombang
3517140	3517	Megaluh
3517150	3517	Tembelang
3517160	3517	Kesamben
3517170	3517	Kudu
3517171	3517	Ngusikan
3517180	3517	Ploso
3517190	3517	Kabuh
3517200	3517	Plandaan
3518010	3518	Sawahan
3518020	3518	Ngetos
3518030	3518	Berbek
3518040	3518	Loceret
3518050	3518	Pace
3518060	3518	Tanjunganom
3518070	3518	Prambon
3518080	3518	Ngronggot
3518090	3518	Kertosono
3518100	3518	Patianrowo
3518110	3518	Baron
3518120	3518	Gondang
3518130	3518	Sukomoro
3518140	3518	Nganjuk
3518150	3518	Bagor
3518160	3518	Wilangan
3518170	3518	Rejoso
3518180	3518	Ngluyu
3518190	3518	Lengkong
3518200	3518	Jatikalen
3519010	3519	Kebonsari
3519020	3519	Geger
3519030	3519	Dolopo
3519040	3519	Dagangan
3519050	3519	Wungu
3519060	3519	Kare
3519070	3519	Gemarang
3519080	3519	Saradan
3519090	3519	Pilangkenceng
3519100	3519	Mejayan
3519110	3519	Wonoasri
3519120	3519	Balerejo
3519130	3519	Madiun
3519140	3519	Sawahan
3519150	3519	Jiwan
3520010	3520	Poncol
3520020	3520	Parang
3520030	3520	Lembeyan
3520040	3520	Takeran
3520041	3520	Nguntoronadi
3520050	3520	Kawedanan
3520060	3520	Magetan
3520061	3520	Ngariboyo
3520070	3520	Plaosan
3520071	3520	Sidorejo
3520080	3520	Panekan
3520090	3520	Sukomoro
3520100	3520	Bendo
3520110	3520	Maospati
3520120	3520	Karangrejo
3520121	3520	Karas
3520130	3520	Barat
3520131	3520	Kartoharjo
3521010	3521	Sine
3521020	3521	Ngrambe
3521030	3521	Jogorogo
3521040	3521	Kendal
3521050	3521	Geneng
3521051	3521	Gerih
3521060	3521	Kwadungan
3521070	3521	Pangkur
3521080	3521	Karangjati
3521090	3521	Bringin
3521100	3521	Padas
3521101	3521	Kasreman
3521110	3521	Ngawi
3521120	3521	Paron
3521130	3521	Kedunggalar
3521140	3521	Pitu
3521150	3521	Widodaren
3521160	3521	Mantingan
3521170	3521	Karanganyar
3522010	3522	Margomulyo
3522020	3522	Ngraho
3522030	3522	Tambakrejo
3522040	3522	Ngambon
3522041	3522	Sekar
3522050	3522	Bumonth
3522051	3522	Gondang
3522060	3522	Temayang
3522070	3522	Sugihwaras
3522080	3522	Kedungadem
3522090	3522	Kepoh Baru
3522100	3522	Baureno
3522110	3522	Kanor
3522120	3522	Sumberejo
3522130	3522	Balen
3522140	3522	Sukosewu
3522150	3522	Kapas
3522160	3522	Bojonegoro
3522170	3522	Trucuk
3522180	3522	Dander
3522190	3522	Ngasem
3522191	3522	Gayam
3522200	3522	Kalitidu
3522210	3522	Malo
3522220	3522	Purwosari
3522230	3522	Padangan
3522240	3522	Kasiman
3522241	3522	Kedewan
3523010	3523	Kenduruan
3523020	3523	Bangilan
3523030	3523	Senori
3523040	3523	Singgahan
3523050	3523	Montong
3523060	3523	Parengan
3523070	3523	Soko
3523080	3523	Rengel
3523081	3523	Grabagan
3523090	3523	Plumpang
3523100	3523	Widang
3523110	3523	Palang
3523120	3523	Semanding
3523130	3523	Tuban
3523140	3523	Jenu
3523150	3523	Merakurak
3523160	3523	Kerek
3523170	3523	Tambakboyo
3523180	3523	Jatirogo
3523190	3523	Bancar
3524010	3524	Sukorame
3524020	3524	Bluluk
3524030	3524	Ngimbang
3524040	3524	Sambeng
3524050	3524	Mantup
3524060	3524	Kembangbahu
3524070	3524	Sugio
3524080	3524	Kedungpring
3524090	3524	Modo
3524100	3524	Babat
3524110	3524	Pucuk
3524120	3524	Sukodadi
3524130	3524	Lamongan
3524140	3524	Tikung
3524141	3524	Sarirejo
3524150	3524	Deket
3524160	3524	Glagah
3524170	3524	Karangbinangun
3524180	3524	Turi
3524190	3524	Kalitengah
3524200	3524	Karang Geneng
3524210	3524	Sekaran
3524220	3524	Maduran
3524230	3524	Laren
3524240	3524	Solokuro
3524250	3524	Paciran
3524260	3524	Brondong
3525010	3525	Wringinanom
3525020	3525	Driyorejo
3525030	3525	Kedamean
3525040	3525	Menganti
3525050	3525	Cerme
3525060	3525	Benjeng
3525070	3525	Balongpanggang
3525080	3525	Duduksampeyan
3525090	3525	Kebomas
3525100	3525	Gresik
3525110	3525	Manyar
3525120	3525	Bungah
3525130	3525	Sidayu
3525140	3525	Dukun
3525150	3525	Panceng
3525160	3525	Ujungpangkah
3525170	3525	Sangkapura
3525180	3525	Tambak
3526010	3526	Kamal
3526020	3526	Labang
3526030	3526	Kwanyar
3526040	3526	Modung
3526050	3526	Blega
3526060	3526	Konang
3526070	3526	Galis
3526080	3526	Tanah Merah
3526090	3526	Tragah
3526100	3526	Socah
3526110	3526	Bangkalan
3526120	3526	Burneh
3526130	3526	Arosbaya
3526140	3526	Geger
3526150	3526	Kokop
3526160	3526	Tanjungbumi
3526170	3526	Sepulu
3526180	3526	Klampis
3527010	3527	Sreseh
3527020	3527	Torjun
3527021	3527	Pangarengan
3527030	3527	Sampang
3527040	3527	Camplong
3527050	3527	Omben
3527060	3527	Kedungdung
3527070	3527	Jrengik
3527080	3527	Tambelangan
3527090	3527	Banyuates
3527100	3527	Robatal
3527101	3527	Karang Penang
3527110	3527	Ketapang
3527120	3527	Sokobanah
3528010	3528	Tlanakan
3528020	3528	Pademawu
3528030	3528	Galis
3528040	3528	Larangan
3528050	3528	Pamekasan
3528060	3528	Proppo
3528070	3528	Palengaan
3528080	3528	Pegantenan
3528090	3528	Kadur
3528100	3528	Pakong
3528110	3528	Waru
3528120	3528	Batu Marmar
3528130	3528	Pasean
3529010	3529	Pragaan
3529020	3529	Bluto
3529030	3529	Saronggi
3529040	3529	Giligenteng
3529050	3529	Talango
3529060	3529	Kalianget
3529070	3529	city Sumenep
3529071	3529	Batuan
3529080	3529	Lenteng
3529090	3529	Ganding
3529100	3529	Guluk Guluk
3529110	3529	Pasongsongan
3529120	3529	Ambunten
3529130	3529	Rubaru
3529140	3529	Dasuk
3529150	3529	Manding
3529160	3529	Batuputih
3529170	3529	Gapura
3529180	3529	Batang Batang
3529190	3529	Dungkek
3529200	3529	Nonggunong
3529210	3529	Gayam
3529220	3529	Raas
3529230	3529	Sapeken
3529240	3529	Arjasa
3529241	3529	Kangayan
3529250	3529	Masalembu
3571010	3571	Mojoroto
3571020	3571	city Kediri
3571030	3571	Pesantren
3572010	3572	Sukorejo
3572020	3572	Kepanjenkidul
3572030	3572	Sananwetan
3573010	3573	Kedungkandang
3573020	3573	Sukun
3573030	3573	Klojen
3573040	3573	Blimbing
3573050	3573	Lowokwaru
3574010	3574	Kademangan
3574011	3574	Kedopok
3574020	3574	Wonoasih
3574030	3574	Mayangan
3574031	3574	Kanigaran
3575010	3575	Gadingrejo
3575020	3575	Purworejo
3575030	3575	Bugulkidul
3575031	3575	Panggungrejo
3576010	3576	Prajurit Kulon
3576020	3576	Magersari
3576021	3576	Kranggan
3577010	3577	Mangu Harjo
3577020	3577	Taman
3577030	3577	Kartoharjo
3578010	3578	Karang Pilang
3578020	3578	timebangan
3578030	3578	Gayungan
3578040	3578	Wonocolo
3578050	3578	Tenggilis Mejoyo
3578060	3578	Gunung Anyar
3578070	3578	Rungkut
3578080	3578	Sukolilo
3578090	3578	Mulyorejo
3578100	3578	Gubeng
3578110	3578	Wonokromo
3578120	3578	Dukuh Pakis
3578130	3578	Wiyung
3578140	3578	Lakarsantri
3578141	3578	Sambikerep
3578150	3578	Tandes
3578160	3578	Suko Manunggal
3578170	3578	Sawahan
3578180	3578	Tegalsari
3578190	3578	Genteng
3578200	3578	Tambaksari
3578210	3578	Kenjeran
3578211	3578	Bulak
3578220	3578	Simokerto
3578230	3578	Semampir
3578240	3578	Pabean Cantian
3578250	3578	Bubutan
3578260	3578	Krembangan
3578270	3578	Asemrowo
3578280	3578	Benowo
3578281	3578	Pakal
3579010	3579	Batu
3579020	3579	Junrejo
3579030	3579	Bumiaji
3601010	3601	Sumur
3601020	3601	Cimanggu
3601030	3601	Cibaliung
3601031	3601	Cibitung
3601040	3601	Cikeusik
3601050	3601	Cigeulis
3601060	3601	Panimbang
3601061	3601	Sobang
3601070	3601	Munjul
3601071	3601	Angsana
3601072	3601	Sindangresmi
3601080	3601	Picung
3601090	3601	Bojong
3601100	3601	Saketi
3601101	3601	Cisata
3601110	3601	Pagelaran
3601111	3601	Patia
3601112	3601	Sukaresmi
3601120	3601	Labuan
3601121	3601	Carita
3601130	3601	Jiput
3601131	3601	Cikedal
3601140	3601	Menes
3601141	3601	Pulosari
3601150	3601	Mandalawangi
3601160	3601	Cimanuk
3601161	3601	Cipeucang
3601170	3601	Banjar
3601171	3601	Kaduhejo
3601172	3601	Mekarjaya
3601180	3601	Pandeglang
3601181	3601	Majasari
3601190	3601	Cadasari
3601191	3601	Karangtanjung
3601192	3601	Koroncong
3602010	3602	Malingping
3602011	3602	Wanasalam
3602020	3602	Panggarangan
3602021	3602	Cihara
3602030	3602	Bayah
3602031	3602	Cilograng
3602040	3602	Cibeber
3602050	3602	Cijaku
3602051	3602	Cigemblong
3602060	3602	Banjarsari
3602070	3602	Cileles
3602080	3602	Gunung Kencana
3602090	3602	Bojongmanik
3602091	3602	Cirinten
3602100	3602	Leuwidamar
3602110	3602	Muncang
3602111	3602	Sobang
3602120	3602	Cipanas
3602121	3602	Lebakgedong
3602130	3602	Sajira
3602140	3602	Cimarga
3602150	3602	Cikulur
3602160	3602	Warunggunung
3602170	3602	Cibadak
3602180	3602	Rangkasbitung
3602181	3602	Kalanganyar
3602190	3602	Maja
3602191	3602	Curugbitung
3603010	3603	Cisoka
3603011	3603	Solear
3603020	3603	Tigaraksa
3603021	3603	timebe
3603030	3603	Cikupa
3603040	3603	Panongan
3603050	3603	Curug
3603051	3603	Kelapa Dua
3603060	3603	Legok
3603070	3603	Pagedangan
3603081	3603	Cisauk
3603120	3603	Pasarkemis
3603121	3603	Sindang Jaya
3603130	3603	Balaraja
3603131	3603	Jayanti
3603132	3603	Sukamulya
3603140	3603	Kresek
3603141	3603	Gunung Kaler
3603150	3603	Kronjo
3603151	3603	Mekar Baru
3603160	3603	Mauk
3603161	3603	Kemiri
3603162	3603	Sukadiri
3603170	3603	Rajeg
3603180	3603	Sepatan
3603181	3603	Sepatan Timur
3603190	3603	Pakuhaji
3603200	3603	Teluknaga
3603210	3603	Kosambi
3604010	3604	Cinangka
3604020	3604	Padarincang
3604030	3604	Ciomas
3604040	3604	Pabuaran
3604041	3604	Gunung Sari
3604050	3604	Baros
3604060	3604	Petir
3604061	3604	Tunjung Teja
3604080	3604	Cikeusal
3604090	3604	Pamarayan
3604091	3604	Bandung
3604100	3604	Jawilan
3604110	3604	Kopo
3604120	3604	Cikande
3604121	3604	Kibin
3604130	3604	Kragilan
3604180	3604	Waringinkurung
3604190	3604	Mancak
3604200	3604	Anyar
3604210	3604	Bojonegara
3604211	3604	Pulo Ampel
3604220	3604	Kramatwatu
3604240	3604	Ciruas
3604250	3604	Pontang
3604251	3604	Lebak Wangi
3604260	3604	Carenang
3604261	3604	Binuang
3604270	3604	Tirtayasa
3604271	3604	Tanara
3671010	3671	Ciledug
3671011	3671	Larangan
3671012	3671	Karang Tengah
3671020	3671	Cipondoh
3671021	3671	Pinang
3671030	3671	Tangerang
3671031	3671	Karawaci
3671040	3671	Jati Uwung
3671041	3671	Cibodas
3671042	3671	Periuk
3671050	3671	Batuceper
3671051	3671	Neglasari
3671060	3671	Benda
3672010	3672	Ciwandan
3672011	3672	Citangkil
3672020	3672	Pulomerak
3672021	3672	Purwakarta
3672022	3672	Grogol
3672030	3672	Cilegon
3672031	3672	Jombang
3672040	3672	Cibeber
3673010	3673	Curug
3673020	3673	Walantaka
3673030	3673	Cipocok Jaya
3673040	3673	Serang
3673050	3673	Taktakan
3673060	3673	Kasemen
3674010	3674	Setu
3674020	3674	Serpong
3674030	3674	Pamulang
3674040	3674	Ciputat
3674050	3674	Ciputat Timur
3674060	3674	Pondok Aren
3674070	3674	Serpong Utara
5101010	5101	Melaya
5101020	5101	Negara
5101021	5101	Jembrana
5101030	5101	Mendoyo
5101040	5101	Pekutatan
5102010	5102	Selemadeg
5102011	5102	Selemadeg Timur
5102012	5102	Selemadeg Barat
5102020	5102	Kerambitan
5102030	5102	Tabanan
5102040	5102	Kediri
5102050	5102	Marga
5102060	5102	Baturiti
5102070	5102	Penebel
5102080	5102	Pupuan
5103010	5103	Kuta Selatan
5103020	5103	Kuta
5103030	5103	Kuta Utara
5103040	5103	Mengwi
5103050	5103	Abiansemal
5103060	5103	Petang
5104010	5104	Sukawati
5104020	5104	Blahbatuh
5104030	5104	Gianyar
5104040	5104	Tampaksiring
5104050	5104	Ubud
5104060	5104	Tegallalang
5104070	5104	Payangan
5105010	5105	Nusapenida
5105020	5105	Banjarangkan
5105030	5105	Klungkung
5105040	5105	Dawan
5106010	5106	Susut
5106020	5106	Bangli
5106030	5106	Tembuku
5106040	5106	Kintamani
5107010	5107	Rendang
5107020	5107	Sidemen
5107030	5107	Manggis
5107040	5107	Karangasem
5107050	5107	Abang
5107060	5107	Bebandem
5107070	5107	Selat
5107080	5107	Kubu
5108010	5108	Gerokgak
5108020	5108	Seririt
5108030	5108	Busungbiu
5108040	5108	Banjar
5108050	5108	Sukasada
5108060	5108	Buleleng
5108070	5108	Sawan
5108080	5108	Kubutambahan
5108090	5108	Tejakula
5171010	5171	Denpasar Selatan
5171020	5171	Denpasar Timur
5171030	5171	Denpasar Barat
5171031	5171	Denpasar Utara
5201010	5201	Sekotong
5201011	5201	Lembar
5201020	5201	Gerung
5201030	5201	Labu Api
5201040	5201	Kediri
5201041	5201	Kuripan
5201050	5201	Narmada
5201051	5201	Lingsar
5201060	5201	Gunung Sari
5201061	5201	Batu Layar
5202010	5202	Praya Barat
5202011	5202	Praya Barat Daya
5202020	5202	Pujut
5202030	5202	Praya Timur
5202040	5202	Janapria
5202050	5202	Kopang
5202060	5202	Praya
5202061	5202	Praya Tengah
5202070	5202	Jonggat
5202080	5202	Pringgarata
5202090	5202	Batukliang
5202091	5202	Batukliang Utara
5203010	5203	Keruak
5203011	5203	Jerowaru
5203020	5203	Sakra
5203021	5203	Sakra Barat
5203022	5203	Sakra Timur
5203030	5203	Terara
5203031	5203	Montong Gading
5203040	5203	Sikur
5203050	5203	Masbagik
5203051	5203	Pringgasela
5203060	5203	Sukamulia
5203061	5203	Suralaga
5203070	5203	Selong
5203071	5203	Labuhan Haji
5203080	5203	Pringgabaya
5203081	5203	Suela
5203090	5203	Aikmel
5203091	5203	Wanasaba
5203092	5203	Sembalun
5203100	5203	Sambelia
5204020	5204	Lunyuk
5204021	5204	Orong Telu
5204050	5204	Alas
5204051	5204	Alas Barat
5204052	5204	Buer
5204061	5204	Utan
5204062	5204	Rhee
5204070	5204	Batulanteh
5204080	5204	Sumbawa
5204081	5204	Labuhan Badas
5204082	5204	Unter Iwes
5204090	5204	Moyohilir
5204091	5204	Moyo Utara
5204100	5204	Moyohulu
5204110	5204	Ropang
5204111	5204	Lenangguar
5204112	5204	Lantung
5204121	5204	Lape
5204122	5204	Lopok
5204130	5204	Plampang
5204131	5204	Labangka
5204132	5204	Maronge
5204140	5204	Empang
5204141	5204	Tarano
5205010	5205	Hu'u
5205011	5205	Pajo
5205020	5205	Dompu
5205030	5205	Woja
5205040	5205	Kilo
5205050	5205	Kempo
5205051	5205	Manggalewa
5205060	5205	Pekat
5206010	5206	Monta
5206011	5206	Parado
5206020	5206	Bolo
5206021	5206	Mada Pangga
5206030	5206	Woha
5206040	5206	Belo
5206041	5206	Palibelo
5206050	5206	Wawo
5206051	5206	Langgudu
5206052	5206	Lambitu
5206060	5206	Sape
5206061	5206	Lambu
5206070	5206	Wera
5206071	5206	Ambalawi
5206080	5206	Donggo
5206081	5206	Soromandi
5206090	5206	Sanggar
5206091	5206	Tambora
5207010	5207	Sekongkang
5207020	5207	Jereweh
5207021	5207	Maluk
5207030	5207	Taliwang
5207031	5207	Brang Ene
5207040	5207	Brang Rea
5207050	5207	Seteluk
5207051	5207	Poto Tano
5208010	5208	Pemenang
5208020	5208	Tanjung
5208030	5208	Gangga
5208040	5208	Kayangan
5208050	5208	Bayan
5271010	5271	Ampenan
5271011	5271	Sekarbela
5271020	5271	Mataram
5271021	5271	Selaparang
5271030	5271	Cakranegara
5271031	5271	Sandubaya
5272010	5272	Rasanae Barat
5272011	5272	Mpunda
5272020	5272	Rasanae Timur
5272021	5272	Raba
5272030	5272	Asacity
5301021	5301	Lamboya
5301022	5301	Wanokaka
5301023	5301	Laboya Barat
5301050	5301	Loli
5301060	5301	city Waikabubak
5301072	5301	Tana Righu
5302010	5302	Lewa
5302011	5302	Nggaha Oriangu
5302012	5302	Lewa Tidahu
5302013	5302	Katala Hamu Lingu
5302020	5302	Tabundung
5302021	5302	Pinupahar
5302030	5302	Paberiwai
5302031	5302	Karera
5302032	5302	Matawai La Pawu
5302033	5302	Kahaungu Eti
5302034	5302	Mahu
5302035	5302	Ngadu Ngala
5302040	5302	Pahunga Lodu
5302041	5302	Wula Waijelu
5302051	5302	Rindi
5302052	5302	Umalulu
5302060	5302	Pandawai
5302061	5302	Kambata Mapambuhang
5302070	5302	city Waingapu
5302071	5302	Kambera
5302080	5302	Haharu
5302081	5302	Kanatang
5303100	5303	Semau
5303101	5303	Semau Selatan
5303110	5303	Kupang Barat
5303111	5303	Nekamese
5303120	5303	Kupang Tengah
5303121	5303	Taebenu
5303130	5303	Amarasi
5303131	5303	Amarasi Barat
5303132	5303	Amarasi Selatan
5303133	5303	Amarasi Timur
5303140	5303	Kupang Timur
5303141	5303	Amabi Oefeto Timur
5303142	5303	Amabi Oefeto
5303150	5303	Sulamu
5303160	5303	Fatuleu
5303161	5303	Fatuleu Tengah
5303162	5303	Fatuleu Barat
5303170	5303	Takari
5303180	5303	Amfoang Selatan
5303181	5303	Amfoang Barat Daya
5303182	5303	Amfoang Tengah
5303190	5303	Amfoang Utara
5303191	5303	Amfoang Barat Laut
5303192	5303	Amfoang Timur
5304010	5304	Mollo Utara
5304011	5304	Fatumnasi
5304012	5304	Tobu
5304013	5304	Nunbena
5304020	5304	Mollo Selatan
5304021	5304	Polen
5304022	5304	Mollo Barat
5304023	5304	Mollo Tengah
5304030	5304	city Soe
5304040	5304	Amanuban Barat
5304041	5304	Batu Putih
5304042	5304	Kuatnana
5304050	5304	Amanuban Selatan
5304051	5304	Noebeba
5304060	5304	Kuan Fatu
5304061	5304	Kualin
5304070	5304	Amanuban Tengah
5304071	5304	Kolbano
5304072	5304	Oenino
5304080	5304	Amanuban Timur
5304081	5304	Fautmolo
5304082	5304	Fatukopa
5304090	5304	Kie
5304091	5304	Kot'olin
5304100	5304	Amanatun Selatan
5304101	5304	Boking
5304102	5304	Nunkolo
5304103	5304	Noebana
5304104	5304	Santian
5304110	5304	Amanatun Utara
5304111	5304	Toianas
5304112	5304	Kokbaun
5305010	5305	Miomaffo Barat
5305011	5305	Miomaffo Tengah
5305012	5305	Musi
5305013	5305	Mutis
5305020	5305	Miomaffo Timur
5305021	5305	Noemuti
5305022	5305	Bikomi Selatan
5305023	5305	Bikomi Tengah
5305024	5305	Bikomi Nilulat
5305025	5305	Bikomi Utara
5305026	5305	Naibenu
5305027	5305	Noemuti Timur
5305030	5305	city Kefamenanu
5305040	5305	Insana
5305041	5305	Insana Utara
5305042	5305	Insana Barat
5305043	5305	Insana Tengah
5305044	5305	Insana Fafinesu
5305050	5305	Biboki Selatan
5305051	5305	Biboki Tanpah
5305052	5305	Biboki Moenleu
5305060	5305	Biboki Utara
5305061	5305	Biboki Anleu
5305062	5305	Biboki Feotleu
5306032	5306	Rai Manuk
5306050	5306	Tasifeto Barat
5306051	5306	Kakuluk Mesak
5306052	5306	Nanaet Dubesi
5306060	5306	Atambua
5306061	5306	Atambua Barat
5306062	5306	Atambua Selatan
5306070	5306	Tasifeto Timur
5306071	5306	Raihat
5306072	5306	Lasiolat
5306080	5306	Lamaknen
5306081	5306	Lamaknen Selatan
5307010	5307	Pantar
5307011	5307	Pantar Barat
5307012	5307	Pantar Timur
5307013	5307	Pantar Barat Laut
5307014	5307	Pantar Tengah
5307020	5307	Alor Barat Daya
5307021	5307	Mataru
5307030	5307	Alor Selatan
5307040	5307	Alor Timur
5307041	5307	Alor Timur Laut
5307042	5307	Pureman
5307050	5307	Teluk Mutiara
5307051	5307	Kabola
5307060	5307	Alor Barat Laut
5307061	5307	Alor Tengah Utara
5307062	5307	Pulau Pura
5307063	5307	overtime
5308010	5308	Nagawutung
5308011	5308	Wulandoni
5308020	5308	Atadei
5308030	5308	Ile Ape
5308031	5308	Ile Ape Timur
5308040	5308	Lebatukan
5308050	5308	Nubatukan
5308060	5308	Omesuri
5308070	5308	Buyasari
5309010	5309	Wulanggitang
5309011	5309	Titehena
5309012	5309	Ilebura
5309020	5309	Tanjung Bunga
5309021	5309	Lewo Lema
5309030	5309	Larantuka
5309031	5309	Ile Mandiri
5309032	5309	Demon Pagong
5309040	5309	Solor Barat
5309041	5309	Solor Selatan
5309050	5309	Solor Timur
5309060	5309	Adonara Barat
5309061	5309	Wotan Ulu Mado
5309062	5309	Adonara Tengah
5309070	5309	Adonara Timur
5309071	5309	Ile Boleng
5309072	5309	Witihama
5309073	5309	Kelubagolit
5309074	5309	Adonara
5310010	5310	Paga
5310011	5310	Mego
5310012	5310	Tana Wawo
5310020	5310	Lela
5310030	5310	Bola
5310031	5310	Doreng
5310032	5310	Mapitara
5310040	5310	Talibura
5310041	5310	Waigete
5310042	5310	Waiblama
5310050	5310	Kewapante
5310051	5310	Hewokloang
5310052	5310	Kangae
5310061	5310	Palue
5310062	5310	Koting
5310063	5310	Nelle
5310070	5310	Nita
5310071	5310	Magepanda
5310080	5310	Alok
5310081	5310	Alok Barat
5310082	5310	Alok Timur
5311010	5311	Nangapanda
5311011	5311	Pulau Ende
5311012	5311	Maukaro
5311020	5311	Ende
5311030	5311	Ende Selatan
5311031	5311	Ende Timur
5311032	5311	Ende Tengah
5311033	5311	Ende Utara
5311040	5311	Ndona
5311041	5311	Ndona Timur
5311050	5311	Wolowaru
5311051	5311	Wolojita
5311052	5311	Lio Timur
5311053	5311	Kelimutu
5311054	5311	Ndori
5311060	5311	Maurole
5311061	5311	citybaru
5311062	5311	Detukeli
5311063	5311	Lepembusu Kelisoke
5311070	5311	Detusoko
5311071	5311	Wewaria
5312010	5312	Aimere
5312011	5312	Jerebuu
5312012	5312	Inerie
5312020	5312	Bajawa
5312030	5312	Golewa
5312031	5312	Golewa Selatan
5312032	5312	Golewa Barat
5312070	5312	Bajawa Utara
5312071	5312	Soa
5312080	5312	Riung
5312081	5312	Riung Barat
5312082	5312	Wolomeze
5313040	5313	Satar Mese
5313041	5313	Satar Mese Barat
5313042	5313	Satar Mese Utara
5313110	5313	Langke Rembong
5313120	5313	Ruteng
5313121	5313	Wae Rii
5313122	5313	Lelak
5313123	5313	Rahong Utara
5313130	5313	Cibal
5313131	5313	Cibal Barat
5313140	5313	Reok
5313141	5313	Reok Barat
5314010	5314	Rote Barat Daya
5314020	5314	Rote Barat Laut
5314030	5314	Lobalain
5314040	5314	Rote Tengah
5314041	5314	Rote Selatan
5314050	5314	Pantai Baru
5314060	5314	Rote Timur
5314061	5314	Landu Leko
5314070	5314	Rote Barat
5314071	5314	Ndao Nuse
5315010	5315	Komodo
5315011	5315	Boleng
5315020	5315	Sano Nggoang
5315021	5315	Mbeliling
5315030	5315	Lembor
5315031	5315	Welak
5315032	5315	Lembor Selatan
5315040	5315	Kuwus
5315041	5315	Ndoso
5315050	5315	Macang Pacar
5316010	5316	Katikutana
5316011	5316	Katikutana Selatan
5316020	5316	Umbu Ratu Nggay Barat
5316030	5316	Umbu Ratu Nggay
5316040	5316	Mamboro
5317010	5317	Kodi Bangedo
5317011	5317	Kodi Balaghar
5317020	5317	Kodi
5317030	5317	Kodi Utara
5317040	5317	Wewewa Selatan
5317050	5317	Wewewa Barat
5317060	5317	Wewewa Timur
5317061	5317	Wewewa Tengah
5317070	5317	Wewewa Utara
5317080	5317	Loura
5317081	5317	city Tambolaka
5318010	5318	Mauponggo
5318020	5318	Keo Tengah
5318030	5318	Nangaroro
5318040	5318	Boawae
5318050	5318	Aesesa Selatan
5318060	5318	Aesesa
5318070	5318	Wolowae
5319010	5319	Borong
5319011	5319	Rana Mese
5319020	5319	city Komba
5319030	5319	Elar
5319031	5319	Elar Selatan
5319040	5319	Sambi Rampas
5319050	5319	Poco Ranaka
5319051	5319	Poco Ranaka Timur
5319060	5319	Lamba Leda
5320010	5320	Raijua
5320020	5320	Hawu Mehara
5320030	5320	Sabu Liae
5320040	5320	Sabu Barat
5320050	5320	Sabu Tengah
5320060	5320	Sabu Timur
5321010	5321	Wewiku
5321020	5321	Malaka Barat
5321030	5321	Weliman
5321040	5321	Rinhat
5321050	5321	Io Kufeu
5321060	5321	Sasita Mean
5321070	5321	Malaka Tengah
5321080	5321	Botin Leobele
5321090	5321	Laen Manen
5321100	5321	Malaka Timur
5321110	5321	Kobalima
5321120	5321	Kobalima Timur
5371010	5371	Alak
5371020	5371	Maulafa
5371030	5371	Oebobo
5371031	5371	city Raja
5371040	5371	Kelapa Lima
5371041	5371	city Lama
6101010	6101	Selakau
6101011	6101	Selakau Timur
6101020	6101	Pemangkat
6101021	6101	Semparuk
6101022	6101	Salatiga
6101030	6101	Tebas
6101031	6101	Tekarang
6101040	6101	Sambas
6101041	6101	Subah
6101042	6101	Sebawi
6101043	6101	Sajad
6101050	6101	Jawai
6101051	6101	Jawai Selatan
6101060	6101	Teluk Keramat
6101061	6101	Galing
6101062	6101	Tangaran
6101070	6101	Sejangkung
6101080	6101	Sajingan Besar
6101090	6101	Paloh
6102010	6102	Sungai Raya
6102011	6102	Capkala
6102012	6102	Sungai Raya Kepulauan
6102030	6102	Samalantan
6102031	6102	Monterado
6102032	6102	Lembah Bawang
6102040	6102	Bengkayang
6102041	6102	Teriak
6102042	6102	Sungai Betung
6102050	6102	Ledo
6102051	6102	Suti Semarang
6102052	6102	Lumar
6102060	6102	Sanggau Ledo
6102061	6102	Tujuhbelas
6102070	6102	Seluas
6102080	6102	Jagoi Babang
6102081	6102	Siding
6103020	6103	Sebangki
6103030	6103	Ngabang
6103031	6103	Jelimpo
6103040	6103	Sengah Temila
6103050	6103	Mandor
6103060	6103	Menjalin
6103070	6103	Mempawah Hulu
6103071	6103	Sompak
6103080	6103	Menyuke
6103081	6103	Banyuke Hulu
6103090	6103	Meranti
6103100	6103	Kuala Behe
6103110	6103	Air Besar
6104080	6104	Siantan
6104081	6104	Segedong
6104090	6104	Sungai Pinyuh
6104091	6104	Anjongan
6104100	6104	Mempawah Hilir
6104101	6104	Mempawah Timur
6104110	6104	Sungai Kunyit
6104120	6104	Toho
6104121	6104	Sadaniang
6105010	6105	Toba
6105020	6105	Meliau
6105060	6105	Kapuas
6105070	6105	Mukok
6105120	6105	Jangkang
6105130	6105	Bonti
6105140	6105	Parindu
6105150	6105	Tayan Hilir
6105160	6105	Balai
6105170	6105	Tayan Hulu
6105180	6105	Kembayan
6105190	6105	Beduwai
6105200	6105	Noyan
6105210	6105	Sekayam
6105220	6105	Entikong
6106010	6106	Kendawangan
6106020	6106	Manis Mata
6106030	6106	Marau
6106031	6106	Singkup
6106032	6106	Air Upas
6106040	6106	Jelai Hulu
6106050	6106	Tumbang Titi
6106051	6106	Pemahan
6106052	6106	Sungai Melayu Rayak
6106060	6106	Matan Hilir Selatan
6106061	6106	Benua Kayong
6106070	6106	Matan Hilir Utara
6106071	6106	Delta Pawan
6106072	6106	Muara Pawan
6106090	6106	Nanga Tayap
6106100	6106	Sandai
6106101	6106	Hulu Sungai
6106110	6106	Sungai Laur
6106120	6106	Simpang Hulu
6106121	6106	Simpang Dua
6107060	6107	Serawai
6107070	6107	Ambalau
6107080	6107	Kayan Hulu
6107110	6107	Sepauk
6107120	6107	Tempunak
6107130	6107	Sungai Tebelian
6107140	6107	Sintang
6107150	6107	Dedai
6107160	6107	Kayan Hilir
6107170	6107	Kelam Permai
6107180	6107	Binjai Hulu
6107190	6107	Ketungau Hilir
6107200	6107	Ketungau Tengah
6107210	6107	Ketungau Hulu
6108010	6108	Silat Hilir
6108020	6108	Silat Hulu
6108030	6108	Hulu Gurung
6108040	6108	Bunut Hulu
6108050	6108	Mentebah
6108060	6108	Bika
6108070	6108	Kalis
6108080	6108	Putussibau Selatan
6108090	6108	Embaloh Hilir
6108100	6108	Bunut Hilir
6108110	6108	Boyan Tanjung
6108120	6108	Pengkadan
6108130	6108	Jongkong
6108140	6108	Selimbau
6108150	6108	Suhaid
6108160	6108	Seberuang
6108170	6108	Semitau
6108180	6108	Empanang
6108190	6108	Puring Kencana
6108200	6108	Badau
6108210	6108	Batang Lupar
6108220	6108	Embaloh Hulu
6108230	6108	Putussibau Utara
6109010	6109	Nanga Mahap
6109020	6109	Nanga Taman
6109030	6109	Sekadau Hulu
6109040	6109	Sekadau Hilir
6109050	6109	Belitang Hilir
6109060	6109	Belitang
6109070	6109	Belitang Hulu
6110010	6110	Sokan
6110020	6110	Tanah Pinoh
6110021	6110	Tanah Pinoh Barat
6110030	6110	Sayan
6110040	6110	Belimbing
6110041	6110	Belimbing Hulu
6110050	6110	Nanga Pinoh
6110051	6110	Pinoh Selatan
6110052	6110	Pinoh Utara
6110060	6110	Ella Hilir
6110070	6110	Menukung
6111010	6111	Pulau Maya
6111011	6111	Kepulauan Karimata
6111020	6111	Sukadana
6111030	6111	Simpang Hilir
6111040	6111	Teluk Batang
6111050	6111	Seponti
6112010	6112	Batu Ampar
6112020	6112	Terentang
6112030	6112	Kubu
6112040	6112	Telok Pa'kedai
6112050	6112	Sungai Kakap
6112060	6112	Rasau Jaya
6112070	6112	Sungai Raya
6112080	6112	Sungai Ambawang
6112090	6112	Kuala Mandor-B
6171010	6171	Pontianak Selatan
6171011	6171	Pontianak Tenggara
6171020	6171	Pontianak Timur
6171030	6171	Pontianak Barat
6171031	6171	Pontianak city
6171040	6171	Pontianak Utara
6172010	6172	Singkawang Selatan
6172020	6172	Singkawang Timur
6172030	6172	Singkawang Utara
6172040	6172	Singkawang Barat
6172050	6172	Singkawang Tengah
6201040	6201	citywaringin Lama
6201050	6201	Arut Selatan
6201060	6201	Kumai
6201061	6201	Pangkalan Banteng
6201062	6201	Pangkalan Lada
6201070	6201	Arut Utara
6202020	6202	Mentaya Hilir Selatan
6202021	6202	Teluk Sampit
6202050	6202	Pulau Hanaut
6202060	6202	Mentawa Baru/Ketapang
6202061	6202	Seranau
6202070	6202	Mentaya Hilir Utara
6202110	6202	city Besi
6202111	6202	Telawang
6202120	6202	Baamang
6202190	6202	Cempaga
6202191	6202	Cempaga Hulu
6202200	6202	Parenggean
6202201	6202	Tualan Hulu
6202210	6202	Mentaya Hulu
6202211	6202	Bukit Santuai
6202230	6202	Antang Kalang
6202231	6202	Telaga Antang
6203020	6203	Kapuas Kuala
6203021	6203	Tamban Catur
6203030	6203	Kapuas Timur
6203040	6203	Selat
6203041	6203	Bataguh
6203070	6203	Basarang
6203080	6203	Kapuas Hilir
6203090	6203	Pulau Petak
6203100	6203	Kapuas Murung
6203101	6203	Dadahup
6203110	6203	Kapuas Barat
6203150	6203	Mantangai
6203160	6203	Timpah
6203170	6203	Kapuas Tengah
6203171	6203	Pasak Talawang
6203180	6203	Kapuas Hulu
6203181	6203	Mandau Talawang
6204010	6204	Jenames
6204020	6204	Dusun Hilir
6204030	6204	Karau Kuala
6204040	6204	Dusun Selatan
6204050	6204	Dusun Utara
6204060	6204	Gunung Bintang Awai
6205010	6205	Montallat
6205020	6205	Gunung Timang
6205030	6205	Gunung Purei
6205040	6205	Teweh Timur
6205050	6205	Teweh Tengah
6205051	6205	Teweh  Baru
6205052	6205	Teweh Selatan
6205060	6205	Lahei
6205061	6205	Lahei Barat
6206010	6206	Jelai
6206011	6206	Pantai Lunci
6206020	6206	Sukamara
6206030	6206	Balai Riam
6206031	6206	Permata Kecubung
6207010	6207	Bulik
6207011	6207	Sematu Jaya
6207012	6207	Menthobi Raya
6207013	6207	Bulik Timur
6207020	6207	Lamandau
6207021	6207	Belantikan Raya
6207030	6207	Delang
6207031	6207	Batangkawa
6208010	6208	Seruyan Hilir
6208011	6208	Seruyan Hilir Timur
6208020	6208	Danau Sembuluh
6208021	6208	Seruyan Raya
6208030	6208	Hanau
6208031	6208	Danau Seluluk
6208040	6208	Seruyan Tengah
6208041	6208	Batu Ampar
6208050	6208	Seruyan Hulu
6208051	6208	Suling Tambun
6209010	6209	Katingan Kuala
6209020	6209	Mendawai
6209030	6209	Kamipang
6209040	6209	Tasik Payawan
6209050	6209	Katingan Hilir
6209060	6209	Tewang Sangalang Garing
6209070	6209	Pulau Malan
6209080	6209	Katingan Tengah
6209090	6209	Sanamen Mantikei
6209091	6209	Petak Malai
6209100	6209	Marikit
6209110	6209	Katingan Hulu
6209111	6209	Bukit Raya
6210010	6210	Kahayan Kuala
6210011	6210	Sebangau Kuala
6210020	6210	Pandih Batu
6210030	6210	Maliku
6210040	6210	Kahayan Hilir
6210041	6210	Jabiren Raya
6210050	6210	Kahayan Tengah
6210060	6210	Baname Tingang
6211010	6211	Manuhing
6211011	6211	Manuhing Raya
6211020	6211	Rungan
6211021	6211	Rungan Hulu
6211022	6211	Rungan Barat
6211030	6211	Sepang
6211031	6211	Mihing Raya
6211040	6211	Kurun
6211050	6211	Tewah
6211060	6211	Kahayan Hulu Utara
6211061	6211	Damang Batu
6211062	6211	Miri Manasa
6212010	6212	Benua Lima
6212020	6212	Dusun Timur
6212021	6212	Paju Epat
6212030	6212	Awang
6212040	6212	Patangkep Tutui
6212050	6212	Dusun Tengah
6212051	6212	Raren Batuah
6212052	6212	Paku
6212053	6212	Karusen Janang
6212060	6212	Pematang Karau
6213010	6213	Permata Intan
6213011	6213	Sungai Babuat
6213020	6213	Murung
6213030	6213	Laung Tuhup
6213031	6213	Barito Tuhup Raya
6213040	6213	Tanah Siang
6213041	6213	Tanah Siang Selatan
6213050	6213	Sumber Barito
6213051	6213	Seribu Riam
6213052	6213	Uut Murung
6271010	6271	Pahandut
6271011	6271	Sabangau
6271012	6271	Jekan Raya
6271020	6271	Bukit Batu
6271021	6271	Rakumpit
6301010	6301	Panyipatan
6301020	6301	Takisung
6301030	6301	Kurau
6301031	6301	Bumi Makmur
6301040	6301	Bati - Bati
6301050	6301	Tambang Ulang
6301060	6301	Pelaihari
6301061	6301	Bajuin
6301070	6301	Batu Ampar
6301080	6301	Jorong
6301090	6301	Kintap
6302010	6302	Pulau Sembilan
6302020	6302	Pulau Laut Barat
6302021	6302	Pulau Laut Tanjung Selayar
6302030	6302	Pulau Laut Selatan
6302031	6302	Pulau Laut Kepulauan
6302040	6302	Pulau Laut Timur
6302050	6302	Pulau Sebuku
6302060	6302	Pulau Laut Utara
6302061	6302	Pulau Laut Tengah
6302120	6302	Kelumpang Selatan
6302121	6302	Kelumpang Hilir
6302130	6302	Kelumpang Hulu
6302140	6302	Hampang
6302150	6302	Sungai Durian
6302160	6302	Kelumpang Tengah
6302161	6302	Kelumpang Barat
6302170	6302	Kelumpang Utara
6302180	6302	Pamukan Selatan
6302190	6302	Sampanahan
6302200	6302	Pamukan Utara
6302201	6302	Pamukan Barat
6303010	6303	Aluh - Aluh
6303011	6303	Beruntung Baru
6303020	6303	Gambut
6303030	6303	Kertak Hanyar
6303031	6303	Tatah Makmur
6303040	6303	Sungai Tabuk
6303050	6303	Martapura
6303051	6303	Martapura Timur
6303052	6303	Martapura Barat
6303060	6303	Astambul
6303070	6303	Karang Intan
6303080	6303	Aranio
6303090	6303	Sungai Pinang
6303091	6303	Paramasan
6303100	6303	Pengaron
6303101	6303	Sambung Makmur
6303110	6303	Mataraman
6303120	6303	Simpang Empat
6303121	6303	Telaga Bauntung
6304010	6304	Tabunganen
6304020	6304	Tamban
6304030	6304	Mekar Sari
6304040	6304	Anjir Pasar
6304050	6304	Anjir Muara
6304060	6304	Alalak
6304070	6304	Mandastana
6304071	6304	Jejangkit
6304080	6304	Belawang
6304090	6304	Wanaraya
6304100	6304	Barambai
6304110	6304	Rantau Badauh
6304120	6304	Cerbon
6304130	6304	Bakumpai
6304140	6304	Marabahan
6304150	6304	Tabukan
6304160	6304	Kuripan
6305010	6305	Binuang
6305011	6305	Hatungun
6305020	6305	Tapin Selatan
6305021	6305	Salam Babaris
6305030	6305	Tapin Tengah
6305040	6305	Bungur
6305050	6305	Piani
6305060	6305	Lokpaikat
6305070	6305	Tapin Utara
6305080	6305	Bakarangan
6305090	6305	Candi Laras Selatan
6305100	6305	Candi Laras Utara
6306010	6306	Padang Batung
6306020	6306	Loksado
6306030	6306	Telaga Langsat
6306040	6306	Angkinang
6306050	6306	Kandangan
6306060	6306	Sungai Raya
6306070	6306	Simpur
6306080	6306	Kalumpang
6306090	6306	Daha Selatan
6306091	6306	Daha Barat
6306100	6306	Daha Utara
6307010	6307	Haruyan
6307020	6307	Batu Benawa
6307030	6307	Hantakan
6307040	6307	Batang Alai Selatan
6307041	6307	Batang Alai Timur
6307050	6307	Barabai
6307060	6307	Labuan Amas Selatan
6307070	6307	Labuan Amas Utara
6307080	6307	Pandawan
6307090	6307	Batang Alai Utara
6307091	6307	Limpasu
6308010	6308	Danau Panggang
6308011	6308	Paminggir
6308020	6308	Babirik
6308030	6308	Sungai Pandan
6308031	6308	Sungai Tabukan
6308040	6308	Amuntai Selatan
6308050	6308	Amuntai Tengah
6308060	6308	Banjang
6308070	6308	Amuntai Utara
6308071	6308	Haur Gading
6309010	6309	Banua Lawas
6309020	6309	Pugaan
6309030	6309	Kelua
6309040	6309	Muara Harus
6309050	6309	Tanta
6309060	6309	Tanjung
6309070	6309	Murung Pudak
6309080	6309	Haruai
6309081	6309	Bintang Ara
6309090	6309	Upau
6309100	6309	Muara Uya
6309110	6309	Jaro
6310010	6310	Kusan Hilir
6310020	6310	Sungai Loban
6310030	6310	Satui
6310031	6310	Angsana
6310040	6310	Kusan Hulu
6310041	6310	Kuranji
6310050	6310	Batu Licin
6310051	6310	Karang Bintang
6310052	6310	Simpang Empat
6310053	6310	Mantewe
6311010	6311	Lampihong
6311020	6311	Batu Mandi
6311030	6311	Awayan
6311031	6311	Tebing Tinggi
6311040	6311	Paringin
6311041	6311	Paringin Selatan
6311050	6311	Juai
6311060	6311	Halong
6371010	6371	Banjarmasin Selatan
6371020	6371	Banjarmasin Timur
6371030	6371	Banjarmasin Barat
6371031	6371	Banjarmasin Tengah
6371040	6371	Banjarmasin Utara
6372010	6372	Landasan Ulin
6372011	6372	Liang Anggang
6372020	6372	Cempaka
6372031	6372	Banjar Baru Utara
6372032	6372	Banjar Baru Selatan
6401010	6401	Batu Sopang
6401011	6401	Muara Samu
6401021	6401	Batu Engau
6401022	6401	Tanjung Harapan
6401030	6401	Pasir Belengkong
6401040	6401	Tanah Grogot
6401050	6401	Kuaro
6401060	6401	Long Ikis
6401070	6401	Muara Komam
6401080	6401	Long Kali
6402010	6402	Bongan
6402020	6402	Jempang
6402030	6402	Penyinggahan
6402040	6402	Muara Pahu
6402041	6402	Siluq Ngurai
6402050	6402	Muara Lawa
6402051	6402	Bentian Besar
6402060	6402	Damai
6402061	6402	Nyuatan
6402070	6402	Barong Tongkok
6402071	6402	Linggang Bigung
6402080	6402	Melak
6402081	6402	Sekolaq Darat
6402082	6402	Manor Bulatn
6402090	6402	Long Iram
6402091	6402	Tering
6403010	6403	Semboja
6403020	6403	Muara Jawa
6403030	6403	Sanga-Sanga
6403040	6403	Loa Janan
6403050	6403	Loa Kulu
6403060	6403	Muara Muntai
6403070	6403	Muara Wis
6403080	6403	citybangun
6403090	6403	Tenggarong
6403100	6403	Sebulu
6403110	6403	Tenggarong Seberang
6403120	6403	Anggana
6403130	6403	Muara Badak
6403140	6403	Marang Kayu
6403150	6403	Muara Kaman
6403160	6403	Kenohan
6403170	6403	Kembang Janggut
6403180	6403	Tabang
6404010	6404	Muara Ancalong
6404011	6404	Busang
6404012	6404	Long Mesangat
6404020	6404	Muara Wahau
6404021	6404	Telen
6404022	6404	Kongbeng
6404030	6404	Muara Bengkal
6404031	6404	Batu Ampar
6404040	6404	Sangatta Utara
6404041	6404	Bengalon
6404042	6404	Teluk Pandan
6404043	6404	Sangatta Selatan
6404044	6404	Rantau Pulung
6404050	6404	Sangkulirang
6404051	6404	Kaliorang
6404052	6404	Sandaran
6404053	6404	Kaubun
6404054	6404	Karangan
6405010	6405	Kelay
6405020	6405	Talisayan
6405021	6405	Tabalar
6405030	6405	Biduk Biduk
6405040	6405	Pulau Derawan
6405041	6405	Maratua
6405050	6405	Sambaliung
6405060	6405	Tanjung Redeb
6405070	6405	Gunung Tabur
6405080	6405	Segah
6405090	6405	Teluk Bayur
6405100	6405	Batu Putih
6405110	6405	Biatan
6409010	6409	Babulu
6409020	6409	Waru
6409030	6409	Penatime
6409040	6409	Sepaku
6411010	6411	Laham
6411020	6411	Long Hubung
6411030	6411	Long Bagun
6411040	6411	Long Pahangai
6411050	6411	Long Apari
6471010	6471	Balikpapan Selatan
6471011	6471	Balikpapan city
6471020	6471	Balikpapan Timur
6471030	6471	Balikpapan Utara
6471040	6471	Balikpapan Tengah
6471050	6471	Balikpapan Barat
6472010	6472	Palaran
6472020	6472	Samarinda Ilir
6472021	6472	Samarinda city
6472022	6472	Sambutan
6472030	6472	Samarinda Seberang
6472031	6472	Loa Janan Ilir
6472040	6472	Sungai Kunjang
6472050	6472	Samarinda Ulu
6472060	6472	Samarinda Utara
6472061	6472	Sungai Pinang
6474010	6474	Bontang Selatan
6474020	6474	Bontang Utara
6474030	6474	Bontang Barat
6501010	6501	Sungai Boh
6501020	6501	Kayan Selatan
6501030	6501	Kayan Hulu
6501040	6501	Kayan Hilir
6501050	6501	Pujungan
6501060	6501	Bahau Hulu
6501070	6501	Sungai Tubu
6501080	6501	Malinau Selatan Hulu
6501090	6501	Malinau Selatan
6501100	6501	Malinau Selatan Hilir
6501110	6501	Mentarang
6501120	6501	Mentarang Hulu
6501130	6501	Malinau Utara
6501140	6501	Malinau Barat
6501150	6501	Malinau city
6502010	6502	Peso
6502020	6502	Peso Hilir
6502030	6502	Tanjung Palas Barat
6502040	6502	Tanjung Palas
6502050	6502	Tanjung Selor
6502060	6502	Tanjung Palas Timur
6502070	6502	Tanjung Palas Tengah
6502080	6502	Tanjung Palas Utara
6502090	6502	Sekatak
6502100	6502	Bunyu
6503010	6503	Muruk Rian
6503020	6503	Sesayap
6503030	6503	Betayau
6503040	6503	Sesayap Hilir
6503050	6503	Tana Lia
6504010	6504	Krayan Selatan
6504011	6504	Krayan Tengah
6504020	6504	Krayan
6504021	6504	Krayan Timur
6504022	6504	Krayan Barat
6504030	6504	Lumbis Ogong
6504040	6504	Lumbis
6504050	6504	Sembakung Atulai
6504060	6504	Sembakung
6504070	6504	Sebuku
6504080	6504	Tulin Onsoi
6504090	6504	Sei Menggaris
6504100	6504	Nunukan
6504110	6504	Nunukan Selatan
6504120	6504	Sebatik Barat
6504130	6504	Sebatik
6504140	6504	Sebatik Timur
6504150	6504	Sebatik Tengah
6504160	6504	Sebatik Utara
6571010	6571	Tarakan Timur
6571020	6571	Tarakan Tengah
6571030	6571	Tarakan Barat
6571040	6571	Tarakan Utara
7101021	7101	Dumoga Barat
7101022	7101	Dumoga Utara
7101023	7101	Dumoga Timur
7101024	7101	Dumoga Tengah
7101025	7101	Dumoga Tenggara
7101026	7101	Dumoga
7101060	7101	Lolayan
7101081	7101	Passi Barat
7101082	7101	Passi Timur
7101083	7101	Bilalang
7101090	7101	Poigar
7101100	7101	Bolaang
7101101	7101	Bolaang Timur
7101110	7101	Lolak
7101120	7101	Sangtombolang
7102091	7102	Langowan Timur
7102092	7102	Langowan Barat
7102093	7102	Langowan Selatan
7102094	7102	Langowan Utara
7102110	7102	Tompaso
7102111	7102	Tompaso Barat
7102120	7102	Kawangkoan
7102121	7102	Kawangkoan Barat
7102122	7102	Kawangkoan Utara
7102130	7102	Sonder
7102160	7102	Tombariri
7102161	7102	Tombariri Timur
7102170	7102	Pineleng
7102171	7102	Tombulu
7102172	7102	Mandolang
7102190	7102	Tondano Barat
7102191	7102	Tondano Selatan
7102200	7102	Remboken
7102210	7102	Kakas
7102211	7102	Kakas Barat
7102220	7102	Lembean Timur
7102230	7102	Eris
7102240	7102	Kombi
7102250	7102	Tondano Timur
7102251	7102	Tondano Utara
7103040	7103	Manganitu Selatan
7103041	7103	Tatoareng
7103050	7103	Tamako
7103060	7103	Tabukan Selatan
7103061	7103	Tabukan Selatan Tengah
7103062	7103	Tabukan Selatan Tenggara
7103070	7103	Tabukan Tengah
7103080	7103	Manganitu
7103090	7103	yeara
7103091	7103	yeara Timur
7103092	7103	yeara Barat
7103100	7103	Tabukan Utara
7103101	7103	Nusa Tabukan
7103102	7103	Kepulauan Marore
7103110	7103	Kendahe
7104010	7104	Kabaruan
7104011	7104	Damau
7104020	7104	Lirung
7104021	7104	Salibabu
7104022	7104	Kalongan
7104023	7104	Moronge
7104030	7104	Melonguane
7104031	7104	Melonguane Timur
7104040	7104	Beo
7104041	7104	Beo Utara
7104042	7104	Beo Selatan
7104050	7104	Rainis
7104051	7104	Tampa Na'mma
7104052	7104	Pulutan
7104060	7104	Essang
7104061	7104	Essang Selatan
7104070	7104	Gemeh
7104080	7104	Nanusa
7104081	7104	Miangas
7105010	7105	Modoinding
7105020	7105	Tompaso Baru
7105021	7105	Maesaan
7105070	7105	Ranoyapo
7105080	7105	Motoling
7105081	7105	Kumelembuai
7105082	7105	Motoling Barat
7105083	7105	Motoling Timur
7105090	7105	Sinonsayang
7105100	7105	Tenga
7105111	7105	Amurang
7105112	7105	Amurang Barat
7105113	7105	Amurang Timur
7105120	7105	Tareran
7105121	7105	Sulta
7105130	7105	Tumpaan
7105131	7105	Tatapaan
7106010	7106	Kema
7106020	7106	Kauditan
7106030	7106	Airmadidi
7106040	7106	Kalawat
7106050	7106	Dimembe
7106051	7106	Talawaan
7106060	7106	Wori
7106070	7106	Likupang Barat
7106080	7106	Likupang Timur
7106081	7106	Likupang Selatan
7107010	7107	Sangkub
7107020	7107	Bintauna
7107030	7107	Bolang Itang Timur
7107040	7107	Bolang Itang Barat
7107050	7107	Kaidipang
7107060	7107	Pinogaluman
7108010	7108	Biaro
7108020	7108	Tagulandang Selatan
7108030	7108	Tagulandang
7108040	7108	Tagulandang Utara
7108050	7108	Siau Barat Selatan
7108060	7108	Siau Timur Selatan
7108070	7108	Siau Barat
7108080	7108	Siau Tengah
7108090	7108	Siau Timur
7108100	7108	Siau Barat Utara
7109010	7109	Ratatotok
7109020	7109	Pusomaen
7109030	7109	Belang
7109040	7109	Ratahan
7109041	7109	Pasan
7109042	7109	Ratahan Timur
7109050	7109	Tombatu
7109051	7109	Tombatu Timur
7109052	7109	Tombatu Utara
7109060	7109	Touluaan
7109061	7109	Touluaan Selatan
7109062	7109	Silian Raya
7110010	7110	Posigadan
7110011	7110	Tomini
7110020	7110	Bolang Uki
7110021	7110	Helumo
7110030	7110	Pinolosian
7110040	7110	Pinolosian Tengah
7110050	7110	Pinolosian Timur
7111010	7111	Nuangan
7111011	7111	Motongkad
7111020	7111	Tutuyan
7111030	7111	citybunan
7111040	7111	Modayag
7111041	7111	Mooat
7111050	7111	Modayag Barat
7171010	7171	Malalayang
7171020	7171	Sario
7171021	7171	Wanea
7171030	7171	Wenang
7171031	7171	Tikala
7171032	7171	Paal Dua
7171040	7171	Mapanget
7171051	7171	Singkil
7171052	7171	Tuminting
7171053	7171	Bunaken
7171054	7171	Bunaken Kepulauan
7172010	7172	Madidir
7172011	7172	Matuari
7172012	7172	Girian
7172021	7172	Lembeh Selatan
7172022	7172	Lembeh Utara
7172030	7172	Aertembaga
7172031	7172	Maesa
7172040	7172	Ranowulu
7173010	7173	Tomohon Selatan
7173020	7173	Tomohon Tengah
7173021	7173	Tomohon Timur
7173022	7173	Tomohon Barat
7173030	7173	Tomohon Utara
7174010	7174	citymobagu Selatan
7174020	7174	citymobagu Timur
7174030	7174	citymobagu Barat
7174040	7174	citymobagu Utara
7201030	7201	Totikum
7201031	7201	Totikum Selatan
7201040	7201	Tinangkung
7201041	7201	Tinangkung Selatan
7201042	7201	Tinangkung Utara
7201050	7201	Liang
7201051	7201	Peling Tengah
7201060	7201	Bulagi
7201061	7201	Bulagi Selatan
7201062	7201	Bulagi Utara
7201070	7201	Buko
7201071	7201	Buko Selatan
7202010	7202	Toili
7202011	7202	Toili Barat
7202012	7202	Moilong
7202020	7202	Batui
7202021	7202	Batui Selatan
7202030	7202	Bunta
7202031	7202	Nuhon
7202032	7202	Simpang Raya
7202040	7202	Kintom
7202050	7202	Luwuk
7202051	7202	Luwuk Timur
7202052	7202	Luwuk Utara
7202053	7202	Luwuk Selatan
7202054	7202	Nambo
7202060	7202	Pagimana
7202061	7202	Bualemo
7202062	7202	Lobu
7202070	7202	Lamala
7202071	7202	Masama
7202072	7202	Mantoh
7202080	7202	Balantak
7202081	7202	Balantak Selatan
7202082	7202	Balantak Utara
7203010	7203	Menui Kepulauan
7203020	7203	Bungku Selatan
7203021	7203	Bahodopi
7203022	7203	Bungku Pesisir
7203030	7203	Bungku Tengah
7203031	7203	Bungku Timur
7203040	7203	Bungku Barat
7203041	7203	Bumi Raya
7203042	7203	Wita Ponda
7204010	7204	Pamona Selatan
7204011	7204	Pamona Barat
7204012	7204	Pamona Tenggara
7204020	7204	Lore Selatan
7204021	7204	Lore Barat
7204030	7204	Pamona Pusalemba
7204031	7204	Pamona Timur
7204032	7204	Pamona Utara
7204040	7204	Lore Utara
7204041	7204	Lore Tengah
7204042	7204	Lore Timur
7204043	7204	Lore Peore
7204050	7204	Poso Pesisir
7204051	7204	Poso Pesisir Selatan
7204052	7204	Poso Pesisir Utara
7204060	7204	Lage
7204070	7204	Poso city
7204071	7204	Poso city Utara
7204072	7204	Poso city Selatan
7205041	7205	Rio Pakava
7205051	7205	Pinembani
7205080	7205	Banawa
7205081	7205	Banawa Selatan
7205082	7205	Banawa Tengah
7205090	7205	Labuan
7205091	7205	Tanantovea
7205100	7205	Sindue
7205101	7205	Sindue Tombusabora
7205102	7205	Sindue Tobata
7205120	7205	Sirenja
7205130	7205	Balaesang
7205131	7205	Balaesang Tanjung
7205140	7205	Dampelas
7205160	7205	Sojol
7205161	7205	Sojol Utara
7206010	7206	Dampal Selatan
7206020	7206	Dampal Utara
7206030	7206	Dondo
7206031	7206	Ogodeide
7206032	7206	Basidondo
7206040	7206	Baolan
7206041	7206	Lampasio
7206050	7206	Galang
7206060	7206	Tolitoli Utara
7206061	7206	Dako Pamean
7207010	7207	Lakea
7207011	7207	Biau
7207012	7207	Karamat
7207020	7207	Momunu
7207021	7207	Tiloan
7207030	7207	Bokat
7207031	7207	Bukal
7207040	7207	Bunobogu
7207041	7207	Gadung
7207050	7207	Paleleh
7207051	7207	Paleleh Barat
7208010	7208	Sausu
7208011	7208	Torue
7208012	7208	Balinggi
7208020	7208	Parigi
7208021	7208	Parigi Selatan
7208022	7208	Parigi Barat
7208023	7208	Parigi Utara
7208024	7208	Parigi Tengah
7208030	7208	Ampibabo
7208031	7208	Kasimbar
7208032	7208	Toribulu
7208033	7208	Siniu
7208040	7208	Tinombo
7208041	7208	Tinombo Selatan
7208042	7208	Sidoan
7208050	7208	Tomini
7208051	7208	Mepanga
7208052	7208	Palasa
7208060	7208	Moutong
7208061	7208	Bolano Lambunu
7208062	7208	Taopa
7208063	7208	Bolano
7208064	7208	Ongka Malino
7209010	7209	Tojo Barat
7209020	7209	Tojo
7209030	7209	Ulubongka
7209040	7209	Ampana Tete
7209050	7209	Ampana city
7209051	7209	Ratolindo
7209060	7209	Una - Una
7209061	7209	Batudaka
7209070	7209	Togean
7209080	7209	Walea Kepulauan
7209081	7209	Walea Besar
7209082	7209	Talatako
7210010	7210	Pipikoro
7210020	7210	Kulawi Selatan
7210030	7210	Kulawi
7210040	7210	Lindu
7210050	7210	Nokilalaki
7210060	7210	Palolo
7210070	7210	Gumbasa
7210080	7210	Dolo Selatan
7210090	7210	Dolo Barat
7210100	7210	Tanambulava
7210110	7210	Dolo
7210120	7210	Sigi Biromaru
7210130	7210	Marawola
7210140	7210	Marawola Barat
7210150	7210	Kinovaro
7211010	7211	Bangkurung
7211020	7211	Labobo
7211030	7211	Banggai Utara
7211040	7211	Banggai
7211050	7211	Banggai Tengah
7211060	7211	Banggai Selatan
7211070	7211	Bokan Kepulauan
7212010	7212	Mori Atas
7212020	7212	Lembo
7212030	7212	Lembo Raya
7212040	7212	Petasia Timur
7212050	7212	Petasia
7212060	7212	Petasia Barat
7212070	7212	Mori Utara
7212080	7212	Soyo Jaya
7212090	7212	Bungku Utara
7212100	7212	Mamosalato
7271010	7271	Palu Barat
7271011	7271	Tatanga
7271012	7271	Ulujadi
7271020	7271	Palu Selatan
7271030	7271	Palu Timur
7271031	7271	Mantikulore
7271040	7271	Palu Utara
7271041	7271	Tawaeli
7301010	7301	Pasimarannu
7301011	7301	Pasilambena
7301020	7301	Pasimassunggu
7301021	7301	Takabonerate
7301022	7301	Pasimassunggu Timur
7301030	7301	Bontosikuyu
7301040	7301	Bontoharu
7301041	7301	Benteng
7301042	7301	Bontomanai
7301050	7301	Bontomatene
7301051	7301	Buki
7302010	7302	Gantarang
7302020	7302	Ujung Bulu
7302021	7302	Ujung Loe
7302030	7302	Bonto Bahari
7302040	7302	Bontotiro
7302050	7302	Hero Lange-Lange
7302060	7302	Kajang
7302070	7302	Bulukumpa
7302080	7302	Rilau Ale
7302090	7302	Kindang
7303010	7303	Bissappu
7303011	7303	Uluere
7303012	7303	Sinoa
7303020	7303	Bantaeng
7303021	7303	Eremerasa
7303030	7303	Tompobulu
7303031	7303	Pa'jukukang
7303032	7303	Gantarangkeke
7304010	7304	Bangkala
7304011	7304	Bangkala Barat
7304020	7304	Tamalatea
7304021	7304	Bontoramba
7304030	7304	Binamu
7304031	7304	Turatea
7304040	7304	Batang
7304041	7304	Arungkeke
7304042	7304	Tarowang
7304050	7304	Kelara
7304051	7304	Rumbia
7305010	7305	Mangara Bombang
7305020	7305	Mappakasunggu
7305021	7305	Sanrobone
7305030	7305	Polombangkeng Selatan
7305031	7305	Pattallassang
7305040	7305	Polombangkeng Utara
7305050	7305	Galesong Selatan
7305051	7305	Galesong
7305060	7305	Galesong Utara
7306010	7306	Bontonompo
7306011	7306	Bontonompo Selatan
7306020	7306	Bajeng
7306021	7306	Bajeng Barat
7306030	7306	Pallangga
7306031	7306	Barombong
7306040	7306	Somba Opu
7306050	7306	Bontomarannu
7306051	7306	Pattallassang
7306060	7306	Parangloe
7306061	7306	Manuju
7306070	7306	Tinggimoncong
7306071	7306	Tombolo Pao
7306072	7306	Parigi
7306080	7306	Bungaya
7306081	7306	Bontolempangan
7306090	7306	Tompobulu
7306091	7306	Biringbulu
7307010	7307	Sinjai Barat
7307020	7307	Sinjai Borong
7307030	7307	Sinjai Selatan
7307040	7307	Tellu Limpoe
7307050	7307	Sinjai Timur
7307060	7307	Sinjai Tengah
7307070	7307	Sinjai Utara
7307080	7307	Bulupoddo
7307090	7307	Pulau Sembilan
7308010	7308	Mandai
7308011	7308	Moncongloe
7308020	7308	Maros Baru
7308021	7308	Marusu
7308022	7308	Turikale
7308023	7308	Lau
7308030	7308	Bontoa
7308040	7308	Bantimurung
7308041	7308	Simbang
7308050	7308	Tanralili
7308051	7308	Tompu Bulu
7308060	7308	Camba
7308061	7308	Cenrana
7308070	7308	Mallawa
7309010	7309	Liukang Tangaya
7309020	7309	Liukang Kalmas
7309030	7309	Liukang Tupabbiring
7309031	7309	Liukang Tupabbiring Utara
7309040	7309	Pangkajene
7309041	7309	Minasatene
7309050	7309	Balocci
7309051	7309	Tondong Tallasa
7309060	7309	Bungoro
7309070	7309	Labakkang
7309080	7309	Ma'rang
7309091	7309	Segeri
7309092	7309	Mandalle
7310010	7310	Tanete Riaja
7310011	7310	Pujananting
7310020	7310	Tanete Rilau
7310030	7310	Barru
7310040	7310	Soppeng Riaja
7310041	7310	Balusu
7310050	7310	Mallusetasi
7311010	7311	Bontocani
7311020	7311	Kahu
7311030	7311	Kajuara
7311040	7311	Salomekko
7311050	7311	Tonra
7311060	7311	Patimpeng
7311070	7311	Libureng
7311080	7311	Mare
7311090	7311	Sibulue
7311100	7311	Cina
7311110	7311	Barebbo
7311120	7311	Ponre
7311130	7311	Lappariaja
7311140	7311	Lamuru
7311141	7311	Tellu Limpoe
7311150	7311	Bengo
7311160	7311	Ulaweng
7311170	7311	Palakka
7311180	7311	Awangpone
7311190	7311	Tellu Siattinge
7311200	7311	Amali
7311210	7311	Ajangale
7311220	7311	Dua Boccoe
7311230	7311	Cenrana
7311710	7311	Tanete Riattang Barat
7311720	7311	Tanete Riattang
7311730	7311	Tanete Riattang Timur
7312010	7312	Mario Riwawo
7312020	7312	Lalabata
7312030	7312	Lili Riaja
7312031	7312	Ganra
7312032	7312	Citta
7312040	7312	Lili Rilau
7312050	7312	Donri Donri
7312060	7312	Mario Riawa
7313010	7313	Sabbang Paru
7313020	7313	Tempe
7313030	7313	Pammana
7313040	7313	Bola
7313050	7313	Takkalalla
7313060	7313	Sajoanging
7313061	7313	Penrang
7313070	7313	Majauleng
7313080	7313	Tana Sitolo
7313090	7313	Belawa
7313100	7313	Maniang Pajo
7313101	7313	Gilireng
7313110	7313	Keera
7313120	7313	Pitumpanua
7314010	7314	Panca Lautang
7314020	7314	Tellulimpo E
7314030	7314	Watang Pulu
7314040	7314	Baranti
7314050	7314	Panca Rijang
7314051	7314	Kulo
7314060	7314	Maritengngae
7314061	7314	Watang Sidenreng
7314070	7314	Pitu Riawa
7314080	7314	Duapitue
7314081	7314	Pitu Riase
7315010	7315	Suppa
7315020	7315	Mattirosompe
7315021	7315	Lanrisang
7315030	7315	Mattiro Bulu
7315040	7315	Watang Sawitto
7315041	7315	Paleteang
7315042	7315	Tiroang
7315050	7315	Patampanua
7315060	7315	Cempa
7315070	7315	Duampanua
7315071	7315	Batulappa
7315080	7315	Lembang
7316010	7316	Maiwa
7316011	7316	Bungin
7316020	7316	Enrekang
7316021	7316	Cendana
7316030	7316	Baraka
7316031	7316	Buntu Batu
7316040	7316	Anggeraja
7316041	7316	Malua
7316050	7316	Alla
7316051	7316	Curio
7316052	7316	Masalle
7316053	7316	Baroko
7317010	7317	Larompong
7317011	7317	Larompong Selatan
7317020	7317	Suli
7317021	7317	Suli Barat
7317030	7317	Belopa
7317031	7317	Kamanre
7317032	7317	Belopa Utara
7317040	7317	Bajo
7317041	7317	Bajo Barat
7317050	7317	Bassesangtempe
7317051	7317	Latimojong
7317052	7317	Bassesangtempe Utara
7317060	7317	Bupon
7317061	7317	Ponrang
7317062	7317	Ponrang Selatan
7317070	7317	Bua
7317080	7317	Walenrang
7317081	7317	Walenrang Timur
7317090	7317	Lamasi
7317091	7317	Walenrang Utara
7317092	7317	Walenrang Barat
7317093	7317	Lamasi Timur
7318010	7318	Bonggakaradeng
7318011	7318	Simbuang
7318012	7318	Rano
7318013	7318	Mappak
7318020	7318	Mengkendek
7318021	7318	Gandang Batu Silanan
7318030	7318	Sangalla
7318031	7318	Sangala Selatan
7318032	7318	Sangalla Utara
7318040	7318	Makale
7318041	7318	Makale Selatan
7318042	7318	Makale Utara
7318050	7318	Saluputti
7318051	7318	Bittuang
7318052	7318	Rembon
7318053	7318	Masanda
7318054	7318	Malimbong Balepe
7318061	7318	Rantetayo
7318067	7318	Kurra
7322010	7322	Sabbang
7322020	7322	Baebunta
7322030	7322	Malangke
7322031	7322	Malangke Barat
7322040	7322	Sukamaju
7322050	7322	Bone-Bone
7322051	7322	Tana Lili
7322120	7322	Masamba
7322121	7322	Mappedeceng
7322122	7322	Rampi
7322130	7322	Limbong
7322131	7322	Seko
7325010	7325	Burau
7325020	7325	Wotu
7325030	7325	Tomoni
7325031	7325	Tomoni Timur
7325040	7325	Angkona
7325050	7325	Malili
7325060	7325	Towuti
7325070	7325	Nuha
7325071	7325	Wasuponda
7325080	7325	Mangkutana
7325081	7325	Kalaena
7326010	7326	Sopai
7326020	7326	Kesu
7326030	7326	Sanggalangi
7326040	7326	Buntao
7326050	7326	Rantebua
7326060	7326	Nanggala
7326070	7326	Tondon
7326080	7326	Tallunglipu
7326090	7326	Rantepao
7326100	7326	Tikala
7326110	7326	Sesean
7326120	7326	Balusu
7326130	7326	Sa'dan
7326140	7326	Bengkelekila
7326150	7326	Sesean Suloara
7326160	7326	Kapala Pitu
7326170	7326	Dende Piongan Napo
7326180	7326	Awan Rante Karua
7326190	7326	Rindingalo
7326200	7326	Buntu Pepasan
7326210	7326	Baruppu
7371010	7371	Mariso
7371020	7371	Mamajang
7371030	7371	Tamalate
7371031	7371	Rappocini
7371040	7371	Makassar
7371050	7371	Ujung Pandang
7371060	7371	Wajo
7371070	7371	Bontoala
7371080	7371	Ujung Tanah
7371081	7371	Kepulauan Sangkarrang
7371090	7371	Tallo
7371100	7371	Panakkukang
7371101	7371	Manggala
7371110	7371	Biring Kanaya
7371111	7371	Tamalanrea
7372010	7372	Bacukiki
7372011	7372	Bacukiki Barat
7372020	7372	Ujung
7372030	7372	Soreang
7373010	7373	Wara Selatan
7373011	7373	Sendana
7373020	7373	Wara
7373021	7373	Wara Timur
7373022	7373	Mungkajang
7373030	7373	Wara Utara
7373031	7373	Bara
7373040	7373	Telluwanua
7373041	7373	Wara Barat
7401050	7401	Lasalimu
7401051	7401	Lasalimu Selatan
7401052	7401	Siontapina
7401060	7401	Pasar Wajo
7401061	7401	Wolowa
7401062	7401	Wabula
7401110	7401	Kapontori
7402010	7402	Tongkuno
7402011	7402	Tongkuno Selatan
7402020	7402	Parigi
7402021	7402	Bone
7402022	7402	Marobo
7402030	7402	Kabawo
7402031	7402	Kabangka
7402032	7402	Kontukowuna
7402061	7402	Kontunaga
7402062	7402	Watopute
7402070	7402	Katobu
7402071	7402	Lohia
7402072	7402	Duruka
7402073	7402	Batalaiworu
7402080	7402	Napabalano
7402081	7402	Lasalepa
7402083	7402	Towea
7402090	7402	Wakorumba Selatan
7402091	7402	Pasir Putih
7402092	7402	Pasi Kolaga
7402111	7402	Maligano
7402112	7402	Batukara
7403090	7403	Soropia
7403091	7403	Lalonggasumeeto
7403100	7403	Sampara
7403101	7403	Bondoala
7403102	7403	Besulutu
7403103	7403	Kapoiala
7403104	7403	Anggalomoare
7403105	7403	Morosi
7403130	7403	Lambuya
7403131	7403	Uepai
7403132	7403	Puriala
7403133	7403	Onembute
7403140	7403	Pondidaha
7403141	7403	Wonggeduku
7403142	7403	Amonggedo
7403143	7403	Wonggeduku Barat
7403150	7403	Wawotobi
7403151	7403	Meluhu
7403152	7403	Konawe
7403153	7403	Anggotoa
7403170	7403	Unaaha
7403171	7403	Anggaberi
7403180	7403	Abuki
7403181	7403	Latoma
7403182	7403	Tongauna
7403183	7403	Asinua
7403184	7403	Padangguni
7403185	7403	Tongauna Utara
7403193	7403	Routa
7404010	7404	Watubangga
7404011	7404	Tanggetada
7404012	7404	Toari
7404013	7404	Polinggona
7404020	7404	Pomalaa
7404030	7404	Wundulako
7404031	7404	Baula
7404060	7404	Kolaka
7404061	7404	Latambaga
7404070	7404	Wolo
7404071	7404	Samaturu
7404072	7404	Iwoimendaa
7405010	7405	Tinanggea
7405011	7405	Lalembuu
7405020	7405	Andoolo
7405021	7405	Buke
7405022	7405	Andoolo Barat
7405030	7405	Palangga
7405031	7405	Palangga Selatan
7405032	7405	Baito
7405040	7405	Lainea
7405041	7405	Laeya
7405050	7405	Kolono
7405051	7405	Kolono Timur
7405060	7405	Laonti
7405070	7405	Moramo
7405071	7405	Moramo Utara
7405080	7405	Konda
7405081	7405	Wolasi
7405090	7405	Ranomeeto
7405091	7405	Ranomeeto Barat
7405100	7405	Landono
7405101	7405	Mowila
7405102	7405	Sabulakoa
7405110	7405	Angata
7405111	7405	Benua
7405112	7405	Basala
7406010	7406	Kabaena
7406011	7406	Kabaena Utara
7406012	7406	Kabaena Selatan
7406013	7406	Kabaena Barat
7406020	7406	Kabaena Timur
7406021	7406	Kabaena Tengah
7406030	7406	Rumbia
7406031	7406	Mata Oleo
7406032	7406	Kep. Masaloka Raya
7406033	7406	Rumbia Tengah
7406040	7406	Rarowatu
7406041	7406	Rarowatu Utara
7406042	7406	Mata Usu
7406043	7406	Lantari Jaya
7406050	7406	Poleang Timur
7406051	7406	Poleang Utara
7406052	7406	Poleang Selatan
7406053	7406	Poleang Tenggara
7406060	7406	Poleang
7406061	7406	Poleang Barat
7406062	7406	Tontonunu
7406063	7406	Poleang Tengah
7407010	7407	Binongko
7407011	7407	Togo Binongko
7407020	7407	Tomia
7407021	7407	Tomia Timur
7407030	7407	Kaledupa
7407031	7407	Kaledupa Selatan
7407040	7407	Wangi-Wangi
7407050	7407	Wangi-Wangi Selatan
7408010	7408	Ranteangin
7408011	7408	Lambai
7408012	7408	Wawo
7408020	7408	Lasusua
7408021	7408	Katoi
7408030	7408	Kodeoha
7408031	7408	Tiwu
7408040	7408	Ngapa
7408041	7408	Watunohu
7408050	7408	Pakue
7408051	7408	Pakue Utara
7408052	7408	Pakue Tengah
7408060	7408	Batu Putih
7408061	7408	Porehu
7408062	7408	Tolala
7409100	7409	Bonegunu
7409101	7409	Kambowa
7409110	7409	Wakorumba
7409120	7409	Kulisusu
7409121	7409	Kulisusu Barat
7409122	7409	Kulisusu Utara
7410010	7410	Sawa
7410011	7410	Motui
7410020	7410	Lembo
7410030	7410	Lasolo
7410031	7410	Wawolesea
7410032	7410	Lasolo Kepulauan
7410040	7410	Molawe
7410050	7410	Asera
7410051	7410	Andowia
7410052	7410	Oheo
7410060	7410	Langgikima
7410070	7410	Wiwirano
7410071	7410	Landawe
7411010	7411	Aere
7411020	7411	Lambandia
7411030	7411	Poli-Polia
7411040	7411	Dangia
7411050	7411	Ladongi
7411060	7411	Loea
7411070	7411	Tirawuta
7411080	7411	Lalolae
7411090	7411	Mowewe
7411100	7411	Tinondo
7411110	7411	Uluiwoi
7411120	7411	Ueesi
7412010	7412	Wawonii Tenggara
7412020	7412	Wawonii Timur
7412030	7412	Wawonii Timur Laut
7412040	7412	Wawonii Utara
7412050	7412	Wawonii Selatan
7412060	7412	Wawonii Tengah
7412070	7412	Wawonii Barat
7413010	7413	Tiworo Kepulauan
7413020	7413	Maginti
7413030	7413	Tiworo Tengah
7413040	7413	Tiworo Selatan
7413050	7413	Tiworo Utara
7413060	7413	Lawa
7413070	7413	Sawerigadi
7413080	7413	Barangka
7413090	7413	Wa Daga
7413100	7413	Kusambi
7413110	7413	Napano Kusambi
7414010	7414	Talaga Raya
7414020	7414	Mawasangka
7414030	7414	Mawasangka Tengah
7414040	7414	Mawasangka Timur
7414050	7414	Lakudo
7414060	7414	Gu
7414070	7414	Sangia Wambulu
7415010	7415	Batu Atas
7415020	7415	Lapandewa
7415030	7415	Sampolawa
7415040	7415	Batauga
7415050	7415	Siompu Barat
7415060	7415	Siompu
7415070	7415	Kadatua
7471010	7471	Mandonga
7471011	7471	Baruga
7471012	7471	Puuwatu
7471013	7471	Kadia
7471014	7471	Wua-Wua
7471020	7471	Poasia
7471021	7471	Abeli
7471022	7471	Kambu
7471023	7471	Nambo
7471030	7471	Kendari
7471031	7471	Kendari Barat
7472010	7472	Betoambari
7472011	7472	Murhum
7472012	7472	Batupoaro
7472020	7472	Wolio
7472021	7472	Kokalukuna
7472030	7472	Sorawolio
7472040	7472	Bungi
7472041	7472	Lea-Lea
7501031	7501	Mananggu
7501040	7501	Tilamuta
7501041	7501	Dulupi
7501042	7501	Botumoito
7501050	7501	Paguyaman
7501051	7501	Wonosari
7501052	7501	Paguyaman Pantai
7502010	7502	Batudaa Pantai
7502011	7502	Biluhu
7502020	7502	Batudaa
7502021	7502	Bongomeme
7502022	7502	Tabongo
7502023	7502	Dungaliyo
7502030	7502	Tibawa
7502031	7502	Pulubala
7502040	7502	Boliyohuto
7502041	7502	Mootilango
7502042	7502	Tolangohula
7502043	7502	Asparaga
7502044	7502	Bilato
7502070	7502	Limboto
7502071	7502	Limboto Barat
7502080	7502	Telaga
7502081	7502	Telaga Biru
7502082	7502	Tilango
7502083	7502	Telaga Jaya
7503010	7503	Popayato
7503011	7503	Popayato Barat
7503012	7503	Popayato Timur
7503020	7503	Lemito
7503021	7503	Wanggarasi
7503030	7503	Marisa
7503031	7503	Patilanggio
7503032	7503	Buntulia
7503033	7503	Duhiadaa
7503040	7503	Randangan
7503041	7503	Taluditi
7503050	7503	Paguat
7503051	7503	Dengilo
7504010	7504	Tapa
7504011	7504	monthgo Utara
7504012	7504	monthgo Selatan
7504013	7504	monthgo Timur
7504014	7504	monthgo Ulu
7504020	7504	Kabila
7504021	7504	Botu Pingge
7504022	7504	Tilongkabila
7504030	7504	Suwawa
7504031	7504	Suwawa Selatan
7504032	7504	Suwawa Timur
7504033	7504	Suwawa Tengah
7504034	7504	Pinogu
7504040	7504	Bonepantai
7504041	7504	Kabila Bone
7504042	7504	Bone Raya
7504043	7504	Bone
7504044	7504	Bulawa
7505010	7505	Atinggola
7505011	7505	Gentuma Raya
7505020	7505	Kwandang
7505021	7505	Tomilito
7505022	7505	Ponelo Kepulauan
7505030	7505	Anggrek
7505031	7505	Monano
7505040	7505	Sumalata
7505041	7505	Sumalata Timur
7505050	7505	Tolinggula
7505051	7505	Biau
7571010	7571	city Barat
7571011	7571	Dungingi
7571020	7571	city Selatan
7571021	7571	city Timur
7571022	7571	Hulonthalangi
7571023	7571	Dumbo Raya
7571030	7571	city Utara
7571031	7571	city Tengah
7571032	7571	Sipatana
7601010	7601	Banggae
7601011	7601	Banggae Timur
7601020	7601	Pamboang
7601030	7601	Sendana
7601031	7601	Tammerodo
7601033	7601	Tubo Sendana
7601040	7601	Malunda
7601041	7601	Ulumanda
7602010	7602	Tinambung
7602011	7602	Balanipa
7602012	7602	Limboro
7602020	7602	Tubbi Taramanu
7602021	7602	Alu
7602030	7602	Campalagian
7602031	7602	Luyo
7602040	7602	Wonomulyo
7602041	7602	Mapilli
7602042	7602	Tapango
7602043	7602	Matakali
7602044	7602	B U L O
7602050	7602	Polewali
7602051	7602	Binuang
7602052	7602	Anreapi
7602061	7602	Matangnga
7603010	7603	Sumarorong
7603020	7603	Messawa
7603030	7603	Pana
7603031	7603	Nosu
7603040	7603	Tabang
7603050	7603	Mamasa
7603060	7603	Tanduk Kalua
7603061	7603	Balla
7603070	7603	Sesenapadang
7603071	7603	Tawalian
7603080	7603	Mambi
7603081	7603	Bambang
7603082	7603	Rantebulahan Timur
7603083	7603	Mehalaan
7603090	7603	Aralle
7603091	7603	Buntu Malangka
7603100	7603	Tabulahan
7604010	7604	Tapalang
7604011	7604	Tapalang Barat
7604020	7604	Mamuju
7604022	7604	Simboro
7604023	7604	Balabalakang
7604030	7604	Kalukku
7604031	7604	Papalang
7604032	7604	Sampaga
7604033	7604	Tommo
7604040	7604	Kalumpang
7604041	7604	Bonehau
7605010	7605	Sarudu
7605011	7605	Dapurang
7605012	7605	Duripoku
7605020	7605	Baras
7605021	7605	Bulu Taba
7605022	7605	Lariang
7605030	7605	Pasangkayu
7605031	7605	Tikke Raya
7605032	7605	Pedongga
7605040	7605	Bambalamotu
7605041	7605	Bambaira
7605042	7605	Sarjo
7606010	7606	Pangale
7606020	7606	Budong-Budong
7606030	7606	Tobadak
7606040	7606	Topoyo
7606050	7606	Karossa
8101040	8101	Tanimbar Selatan
8101041	8101	Wer Tamrian
8101042	8101	Wer Maktian
8101043	8101	Selaru
8101050	8101	Tanimbar Utara
8101051	8101	Yaru
8101052	8101	Wuar Labobar
8101053	8101	Nirunmas
8101054	8101	Kormomolin
8101055	8101	Molu Maru
8102010	8102	Kei Kecil
8102012	8102	Kei Kecil Barat
8102013	8102	Kei Kecil Timur
8102014	8102	Hoat Sorbay
8102015	8102	Manyeuw
8102016	8102	Kei Kecil Timur Selatan
8102020	8102	Kei Besar
8102021	8102	Kei Besar Utara Timur
8102022	8102	Kei Besar Selatan
8102023	8102	Kei Besar Utara Barat
8102024	8102	Kei Besar Selatan Barat
8103010	8103	Banda
8103040	8103	Tehoru
8103041	8103	Telutih
8103050	8103	Amahai
8103051	8103	city Masohi
8103052	8103	Teluk Elpaputih
8103060	8103	Teon Nila Serua
8103080	8103	Saparua
8103081	8103	Nusalaut
8103082	8103	Saparua Timur
8103090	8103	P. Haruku
8103100	8103	Salahutu
8103110	8103	Leihitu
8103111	8103	Leihitu Barat
8103140	8103	Seram Utara
8103141	8103	Seram Utara Barat
8103142	8103	Seram Utara Timur Kobi
8103143	8103	Seram Utara Timur Seti
8104020	8104	Namlea
8104021	8104	Waeapo
8104022	8104	Waplau
8104023	8104	Bata Bual
8104024	8104	Teluk Kaiely
8104025	8104	Waelata
8104026	8104	Lolong Guba
8104027	8104	Lilialy
8104030	8104	Air Buaya
8104031	8104	Fena Leisela
8105010	8105	Aru Selatan
8105011	8105	Aru Selatan Timur
8105012	8105	Aru Selatan Utara
8105020	8105	Aru Tengah
8105021	8105	Aru Tengah Timur
8105022	8105	Aru Tengah Selatan
8105030	8105	Pulau-Pulau Aru
8105031	8105	Aru Utara
8105032	8105	Aru Utara Timur Batuley
8105033	8105	Sir-Sir
8106010	8106	Huamual Belakang
8106011	8106	Kepulauan Manipa
8106020	8106	Seram Barat
8106021	8106	Huamual
8106030	8106	Kairatu
8106031	8106	Kairatu Barat
8106032	8106	Inamosol
8106033	8106	Amalatu
8106034	8106	Elpaputih
8106040	8106	Taniwel
8106041	8106	Taniwel Timur
8107010	8107	Pulau Gorom
8107011	8107	Wakate
8107012	8107	Teor
8107013	8107	Gorom Timur
8107014	8107	Pulau Panjang
8107020	8107	Seram Timur
8107021	8107	Tutuk Tolu
8107022	8107	Kilmury
8107023	8107	Lian Vitu
8107024	8107	Kian Darat
8107030	8107	Weriname
8107031	8107	Siwalalat
8107040	8107	Bula
8107041	8107	Bula Barat
8107042	8107	Teluk Waru
8108010	8108	Wetar
8108011	8108	Wetar Barat
8108012	8108	Wetar Utara
8108013	8108	Wetar Timur
8108020	8108	Pp. Terselatan
8108021	8108	Kisar Utara
8108022	8108	Kepulauan Romang
8108030	8108	Letti
8108041	8108	Moa
8108042	8108	Lakor
8108050	8108	Damer
8108060	8108	Mdona Hiera
8108070	8108	Pp. Babar
8108071	8108	Pulau Wetang
8108080	8108	Babar Timur
8108081	8108	Pulau Masela
8108082	8108	Dawelor Dawera
8109010	8109	Kepala Madan
8109020	8109	Leksula
8109021	8109	Fena Fafan
8109030	8109	Namrole
8109040	8109	Waisama
8109050	8109	Ambalau
8171010	8171	Nusaniwe
8171020	8171	Sirimau
8171021	8171	Leitimur Selatan
8171030	8171	Teluk Ambon Baguala
8171031	8171	Teluk Ambon
8172010	8172	Pp. Kur
8172011	8172	Kur Selatan
8172020	8172	Tayando Tam
8172030	8172	Pulau Dullah Utara
8172040	8172	Pulau Dullah Selatan
8201090	8201	Jailolo
8201091	8201	Jailolo Selatan
8201100	8201	Sahu
8201101	8201	Sahu Timur
8201130	8201	Ibu
8201131	8201	Ibu Selatan
8201132	8201	Tabaru
8201140	8201	Loloda
8202030	8202	Weda
8202031	8202	Weda Selatan
8202032	8202	Weda Utara
8202033	8202	Weda Tengah
8202034	8202	Weda Timur
8202041	8202	Pulau Gebe
8202042	8202	Patani
8202043	8202	Patani Utara
8202044	8202	Patani Barat
8202045	8202	Patani Timur
8203010	8203	Sula Besi Barat
8203011	8203	Sulabesi Selatan
8203020	8203	Sanana
8203021	8203	Sula Besi Tengah
8203022	8203	Sulabesi Timur
8203023	8203	Sanana Utara
8203030	8203	Mangoli Timur
8203031	8203	Mangoli Tengah
8203032	8203	Mangoli Utara Timur
8203040	8203	Mangoli Barat
8203041	8203	Mangoli Utara
8203042	8203	Mangoli Selatan
8204010	8204	Obi Selatan
8204020	8204	Obi
8204021	8204	Obi Barat
8204022	8204	Obi Timur
8204023	8204	Obi Utara
8204030	8204	Bacan
8204031	8204	Mandioli Selatan
8204032	8204	Mandioli Utara
8204033	8204	Bacan Selatan
8204034	8204	Batang Lomang
8204040	8204	Bacan Timur
8204041	8204	Bacan Timur Selatan
8204042	8204	Bacan Timur Tengah
8204050	8204	Bacan Barat
8204051	8204	Kasiruta Barat
8204052	8204	Kasiruta Timur
8204053	8204	Bacan Barat Utara
8204060	8204	Kayoa
8204061	8204	Kayoa Barat
8204062	8204	Kayoa Selatan
8204063	8204	Kayoa Utara
8204070	8204	Pulau Makian
8204071	8204	Makian Barat
8204080	8204	Gane Barat
8204081	8204	Gane Barat Selatan
8204082	8204	Gane Barat Utara
8204083	8204	Kepulauan Joronga
8204090	8204	Gane Timur
8204091	8204	Gane Timur Tengah
8204092	8204	Gane Timur Selatan
8205010	8205	Malifut
8205011	8205	Kao Teluk
8205020	8205	Kao
8205021	8205	Kao Barat
8205022	8205	Kao Utara
8205030	8205	Tobelo Selatan
8205031	8205	Tobelo Barat
8205032	8205	Tobelo Timur
8205040	8205	Tobelo
8205041	8205	Tobelo Tengah
8205042	8205	Tobelo Utara
8205050	8205	Galela
8205051	8205	Galela Selatan
8205052	8205	Galela Barat
8205053	8205	Galela Utara
8205060	8205	Loloda Utara
8205061	8205	Loloda Kepulauan
8206010	8206	Maba Selatan
8206011	8206	city Maba
8206020	8206	Wasile Selatan
8206030	8206	Wasile
8206031	8206	Wasile Timur
8206032	8206	Wasile Tengah
8206033	8206	Wasile Utara
8206040	8206	Maba
8206041	8206	Maba Tengah
8206042	8206	Maba Utara
8207010	8207	Morotai Selatan
8207020	8207	Morotai Timur
8207030	8207	Morotai Selatan Barat
8207040	8207	Morotai Jaya
8207050	8207	Morotai Utara
8208010	8208	Taliabu Barat
8208020	8208	Taliabu Selatan
8208030	8208	Tabona
8208040	8208	Taliabu Timur Selatan
8208050	8208	Taliabu Timur
8208060	8208	Taliabu Utara
8208070	8208	Lede
8208080	8208	Taliabu Barat Laut
8271010	8271	Pulau Ternate
8271011	8271	Moti
8271012	8271	Pulau Batang Dua
8271013	8271	Pulau Hiri
8271014	8271	Ternate Barat
8271020	8271	Ternate Selatan
8271021	8271	Ternate Tengah
8271030	8271	Ternate Utara
8272010	8272	Tidore Selatan
8272020	8272	Tidore Utara
8272030	8272	Tidore
8272031	8272	Tidore Timur
8272040	8272	Oba
8272041	8272	Oba Selatan
8272050	8272	Oba Utara
8272051	8272	Oba Tengah
9101050	9101	Fakfak Timur
9101051	9101	Karas
9101052	9101	Fakfak Timur Tengah
9101060	9101	Fakfak
9101061	9101	Fakfak Tengah
9101062	9101	Pariwari
9101070	9101	Fakfak Barat
9101071	9101	Wartutin
9101080	9101	Kokas
9101081	9101	Teluk Patipi
9101082	9101	Kramongmongga
9101083	9101	Bomberay
9101084	9101	Arguni
9101085	9101	Mbahamdandara
9101086	9101	Furwagi
9101087	9101	Kayauni
9101088	9101	Tomage
9102010	9102	Buruway
9102020	9102	Teluk Arguni
9102021	9102	Teluk Arguni Bawah
9102030	9102	Kaimana
9102031	9102	Kambrau
9102040	9102	Teluk Etna
9102041	9102	Yamor
9103010	9103	Naikere
9103020	9103	Wondiboy
9103021	9103	Rasiey
9103022	9103	Kuri Wamesa
9103030	9103	Wasior
9103040	9103	Duairi
9103041	9103	Roon
9103050	9103	Windesi
9103051	9103	Nikiwar
9103060	9103	Wamesa
9103061	9103	Roswar
9103070	9103	Rumberpon
9103071	9103	Soug Jaya
9104010	9104	Fafurwar
9104020	9104	Babo
9104021	9104	Sumuri
9104022	9104	Aroba
9104023	9104	Kaitaro
9104030	9104	Kuri
9104040	9104	Wamesa
9104050	9104	Bintuni
9104051	9104	Manimeri
9104052	9104	Tuhiba
9104053	9104	Dataran Beimes
9104060	9104	Tembuni
9104070	9104	Aranday
9104071	9104	Kamundan
9104072	9104	Tomu
9104073	9104	Weriagar
9104080	9104	Moskona Selatan
9104081	9104	Meyado
9104082	9104	Moskona Barat
9104090	9104	Merdey
9104091	9104	Biscoop
9104092	9104	Masyeta
9104100	9104	Moskona Utara
9104101	9104	Moskona Timur
9105110	9105	Warmare
9105120	9105	Prafi
9105141	9105	Manokwari Barat
9105142	9105	Manokwari Timur
9105143	9105	Manokwari Utara
9105144	9105	Manokwari Selatan
9105146	9105	Tanah Rubu
9105170	9105	Masni
9105171	9105	Sidey
9106010	9106	Inanwatan
9106011	9106	Metemani
9106020	9106	Kokoda
9106021	9106	Kais
9106022	9106	Kokoda Utara
9106023	9106	Kais Darat
9106060	9106	Moswaren
9106070	9106	Teminabuan
9106071	9106	Seremuk
9106072	9106	Wayer
9106073	9106	Konda
9106074	9106	Saifi
9106080	9106	Sawiat
9106081	9106	Fokour
9106082	9106	Salkma
9107061	9107	Klaso
9107062	9107	Saengkeduk
9107100	9107	Makbon
9107101	9107	Klayili
9107110	9107	Beraur
9107111	9107	Klamono
9107112	9107	Klabot
9107113	9107	Klawak
9107114	9107	Bagun
9107115	9107	Klasafet
9107116	9107	Malabotom
9107118	9107	Botain
9107119	9107	Konhir
9107120	9107	Salawati
9107121	9107	Mayamuk
9107122	9107	Salawati Timur
9107123	9107	Hobard
9107124	9107	Buk
9107130	9107	Seget
9107131	9107	Segun
9107132	9107	Salawati Selatan
9107133	9107	Salawati Tengah
9107170	9107	Aimas
9107171	9107	Mariat
9107172	9107	Sorong
9107180	9107	Sayosa
9107181	9107	Maudus
9107182	9107	Wemak
9107183	9107	Sayosa Timur
9107184	9107	Sunook
9108011	9108	Misool Selatan
9108012	9108	Misool Barat
9108020	9108	Misool
9108021	9108	Kofiau
9108022	9108	Misool Timur
9108023	9108	Kepulauan Sembilan
9108031	9108	Salawati Utara
9108033	9108	Salawati Tengah
9108034	9108	Salawati Barat
9108035	9108	Batanta Selatan
9108036	9108	Batanta Utara
9108040	9108	Waigeo Selatan
9108041	9108	Teluk Mayalibit
9108042	9108	Meos Mansar
9108043	9108	city Waisai
9108044	9108	Tiplol Mayalibit
9108050	9108	Waigeo Barat
9108051	9108	Waigeo Barat Kepulauan
9108060	9108	Waigeo Utara
9108061	9108	Warwarbomi
9108062	9108	Supnin
9108070	9108	Kepulauan Ayau
9108071	9108	Ayau
9108080	9108	Waigeo Timur
9109010	9109	Fef
9109011	9109	Syujak
9109012	9109	Ases
9109013	9109	Tinggouw
9109020	9109	Miyah
9109021	9109	Miyah Selatan
9109022	9109	Ireres
9109023	9109	Wilhem Roumbouts
9109030	9109	Abun
9109040	9109	Kwoor
9109041	9109	Tobouw
9109042	9109	Kwesefo
9109050	9109	Sausapor
9109051	9109	Bikar
9109060	9109	Yembun
9109061	9109	Bamusbama
9109070	9109	Kebar
9109071	9109	Kebar Timur
9109072	9109	Kebar Selatan
9109073	9109	Manekar
9109080	9109	Senopi
9109081	9109	Mawabuan
9109090	9109	Amberbaken
9109091	9109	Mpur
9109092	9109	Amberbaken Barat
9109100	9109	Mubarni / Arfu
9109110	9109	Moraid
9109111	9109	Selemkai
9110010	9110	Aitinyo Barat/Athabu
9110011	9110	Ayamaru Selatan Jaya
9110020	9110	Aitinyo
9110021	9110	Aitinyo Tengah
9110030	9110	Aifat Selatan
9110031	9110	Aifat Timur Selatan
9110040	9110	Aifat
9110050	9110	Aitinyo Utara
9110051	9110	Aitinyo Raya
9110060	9110	Ayamaru Timur
9110061	9110	Ayamaru Timur Selatan
9110070	9110	Ayamaru
9110071	9110	Ayamaru Selatan
9110072	9110	Ayamaru Jaya
9110073	9110	Ayamaru Tengah
9110074	9110	Ayamaru Barat
9110080	9110	Ayamaru Utara
9110081	9110	Ayamaru Utara Timur
9110090	9110	Mare
9110091	9110	Mare Selatan
9110100	9110	Aifat Utara
9110110	9110	Aifat Timur
9110111	9110	Aifat Timur Tengah
9110112	9110	Aifat Timur Jauh
9111010	9111	Tahota
9111020	9111	Dataran Isim
9111030	9111	Nenei
9111040	9111	Momi Waren
9111050	9111	Ransiki
9111060	9111	Oransbari
9112010	9112	Didohu
9112020	9112	Sururey
9112030	9112	Anggi Gida
9112040	9112	Membey
9112050	9112	Anggi
9112060	9112	Taige
9112070	9112	Hingk
9112080	9112	Menyambouw
9112090	9112	Catubouw
9112100	9112	Testega
9171010	9171	Sorong Barat
9171011	9171	Sorong Kepulauan
9171012	9171	Maladum Mes
9171020	9171	Sorong Timur
9171021	9171	Sorong Utara
9171022	9171	Sorong
9171023	9171	Sorong Manoi
9171024	9171	Klaurung
9171025	9171	Malaimsimsa
9171026	9171	Sorong city
9401010	9401	Kimaam
9401011	9401	Waan
9401012	9401	Tabonji
9401013	9401	Ilwayab
9401020	9401	Okaba
9401021	9401	Tubang
9401022	9401	Ngguti
9401023	9401	Kaptel
9401030	9401	Kurik
9401031	9401	Malind
9401032	9401	Animha
9401040	9401	Merauke
9401041	9401	Semangga
9401042	9401	Tanah Miring
9401043	9401	Jagebob
9401044	9401	Sota
9401045	9401	Naukenjerai
9401050	9401	Muting
9401051	9401	Eligobel
9401052	9401	Ulilin
9402110	9402	Wamena
9402111	9402	Asolokobal
9402112	9402	Walelagama
9402113	9402	Trikora
9402114	9402	Napua
9402115	9402	Walaik
9402116	9402	Wouma
9402117	9402	Walesi
9402118	9402	Asotipo
9402119	9402	Maima
9402120	9402	Hubikosi
9402121	9402	Pelebaga
9402122	9402	Ibele
9402123	9402	Tailarek
9402124	9402	Hubikiak
9402180	9402	Asologaima
9402181	9402	Musatfak
9402182	9402	Silo Karno Doga
9402183	9402	Pyramid
9402184	9402	Muliama
9402185	9402	Wame
9402190	9402	Kurulu
9402191	9402	Usilimo
9402192	9402	Wita Waya
9402193	9402	Libarek
9402194	9402	Wadangku
9402195	9402	Pisugi
9402220	9402	Bolakme
9402221	9402	Wollo
9402222	9402	Yalengga
9402223	9402	Tagime
9402224	9402	Molagalome
9402225	9402	Tagineri
9402226	9402	Bugi
9402227	9402	Bpiri
9402228	9402	Koragi
9402611	9402	Itlay Hasige
9402612	9402	Siepkosi
9402614	9402	Popugoba
9403080	9403	Kaureh
9403081	9403	Airu
9403082	9403	Yapsi
9403140	9403	Kemtuk
9403150	9403	Kemtuk Gresi
9403151	9403	Gresi Selatan
9403160	9403	Nimboran
9403161	9403	Nimboran Timur / Namblong
9403170	9403	Nimbokrang
9403180	9403	Unurum Guay
9403200	9403	Demta
9403201	9403	Yokari
9403210	9403	Depapre
9403211	9403	Ravenirara
9403220	9403	Sentani Barat
9403221	9403	Waibu
9403230	9403	Sentani
9403231	9403	Ebungfau
9403240	9403	Sentani Timur
9404050	9404	Uwapa
9404051	9404	Menou
9404052	9404	Dipa
9404060	9404	Yaur
9404061	9404	Teluk Umar
9404070	9404	Wanggar
9404071	9404	Nabire Barat
9404080	9404	Nabire
9404081	9404	Teluk Kimi
9404090	9404	Napan
9404091	9404	Makimi
9404092	9404	Wapoga
9404093	9404	Kepulauan Moora
9404100	9404	Siriwo
9404110	9404	Yaro
9408040	9408	Yapen Timur
9408041	9408	Pantura Yapen
9408042	9408	Teluk Ampimoi
9408043	9408	Raimbawi
9408044	9408	Pulau Kurudu
9408050	9408	Angkaisera
9408051	9408	Kep. Ambai
9408052	9408	Yawakukat
9408060	9408	Yapen Selatan
9408061	9408	Kosiwo
9408062	9408	Anataurei
9408070	9408	Yapen Barat
9408071	9408	Wonawa
9408072	9408	Pulau Yerui
9408080	9408	Poom
9408081	9408	Windesi
9409010	9409	Numfor Barat
9409011	9409	Orkeri
9409020	9409	Numfor Timur
9409021	9409	Bruyadori
9409022	9409	Poiru
9409030	9409	Padaido
9409031	9409	Aimando Padaido
9409040	9409	Biak Timur
9409041	9409	Oridek
9409050	9409	Biak city
9409060	9409	Samofa
9409070	9409	Yendidori
9409080	9409	Biak Utara
9409081	9409	Andey
9409090	9409	Warsa
9409091	9409	Yawosi
9409092	9409	Bondifuar
9409100	9409	Biak Barat
9409101	9409	Swandiwe
9410030	9410	Paniai Timur
9410031	9410	Yatamo
9410032	9410	Kebo
9410033	9410	Pugo Dagi
9410034	9410	Wege Muka
9410035	9410	Wegee Bino
9410036	9410	Yagai
9410040	9410	Bibida
9410041	9410	Dumadama
9410070	9410	Aradide
9410071	9410	Ekadide
9410072	9410	Aweida
9410073	9410	Fajar Timur
9410074	9410	Topiyai
9410080	9410	Paniai Barat
9410081	9410	Siriwo
9410082	9410	Muye
9410083	9410	Nakama
9410084	9410	Teluk Deya
9410090	9410	Bogobaida
9411040	9411	Fawi
9411041	9411	Dagai
9411042	9411	Kiyage
9411050	9411	Mulia
9411053	9411	Yambi
9411054	9411	Ilamburawi
9411055	9411	Muara
9411056	9411	Pagaleme
9411057	9411	Gurage
9411058	9411	Irimuli
9411060	9411	Ilu
9411061	9411	Torere
9411063	9411	Yamoneri
9411064	9411	Waegi
9411065	9411	Nume
9411066	9411	Nioga
9411067	9411	Gubume
9411068	9411	Taganombak
9411070	9411	Tingginambut
9411071	9411	Kalome
9411072	9411	Wanwi
9411080	9411	Mewoluk
9411081	9411	Lumo
9411082	9411	Molanikime
9411090	9411	Yamo
9411091	9411	Dokome
9412010	9412	Mimika Barat
9412011	9412	Mimika Barat Jauh
9412012	9412	Mimika Barat Tengah
9412013	9412	Amar
9412020	9412	Mimika Timur
9412021	9412	Mimika Tengah
9412022	9412	Mimika Timur Jauh
9412030	9412	Mimika Baru
9412031	9412	Kuala Kencana
9412032	9412	Tembagapura
9412033	9412	Wania
9412034	9412	Iwaka
9412035	9412	Kwamki Narama
9412040	9412	Agimuga
9412041	9412	Jila
9412042	9412	Jita
9412043	9412	Alama
9412044	9412	Hoya
9413010	9413	Jair
9413011	9413	Subur
9413013	9413	Kia
9413020	9413	Mindiptana
9413021	9413	Iniyandit
9413022	9413	Kombut
9413023	9413	Sesnuk
9413030	9413	Mandobo
9413031	9413	Fofi
9413032	9413	Arimop
9413040	9413	Kouh
9413041	9413	Bomakia
9413042	9413	Firiwage
9413043	9413	Manggelum
9413044	9413	Yaniruma
9413045	9413	Kawagit
9413046	9413	Kombay
9413050	9413	Waropko
9413051	9413	Ambatkwi
9413052	9413	Ninati
9414010	9414	Nambioman Bapai
9414011	9414	Minyamur
9414020	9414	Edera
9414021	9414	Venaha
9414022	9414	Syahcame
9414023	9414	Bamgi
9414024	9414	Yakomi
9414030	9414	Obaa
9414031	9414	Passue
9414040	9414	Haju
9414050	9414	Assue
9414060	9414	Citakmitak
9414061	9414	Kaibar
9414062	9414	Passue Bawah
9414063	9414	Ti-Zain
9415010	9415	Pantai Kasuari
9415011	9415	Kopay
9415012	9415	Der Koumur
9415013	9415	Safan
9415014	9415	Awyu
9415020	9415	Fayit
9415021	9415	Aswi
9415030	9415	Atsy
9415031	9415	Sirets
9415032	9415	Ayip
9415033	9415	Bectbamu
9415040	9415	Suator
9415041	9415	Kolf Braza
9415042	9415	Joutu
9415043	9415	Koroway Buluanop
9415050	9415	Akat
9415051	9415	Jetsy
9415060	9415	Agats
9415070	9415	Sawa Erma
9415071	9415	Suru-Suru
9415072	9415	Unir Sirau
9415073	9415	Joerat
9415074	9415	Pulau Tiga
9416010	9416	Kurima
9416011	9416	Musaik
9416013	9416	Dekai
9416014	9416	Obio
9416015	9416	Pasema
9416016	9416	Amuma
9416017	9416	Suru-Suru
9416018	9416	Wusama
9416019	9416	Silimo
9416020	9416	Ninia
9416021	9416	Holuwon
9416022	9416	Lolat
9416023	9416	Langda
9416024	9416	Bomela
9416025	9416	Suntamon
9416026	9416	Sobaham
9416027	9416	Korupun
9416028	9416	Sela
9416029	9416	Kwelamdua
9416030	9416	Anggruk
9416031	9416	Panggema
9416032	9416	Walma
9416033	9416	Kosarek
9416034	9416	Ubahak
9416035	9416	Nalca
9416036	9416	Puldama
9416037	9416	Nipsan
9416041	9416	Samenage
9416042	9416	Tangma
9416043	9416	Soba
9416044	9416	Mugi
9416045	9416	Yogosem
9416046	9416	Kayo
9416047	9416	Sumo
9416048	9416	Hogio
9416049	9416	Ukha
9416051	9416	Werima
9416052	9416	Soloikma
9416053	9416	Seradala
9416054	9416	Kabianggama
9416055	9416	Kwikma
9416056	9416	Hilipuk
9416057	9416	Yahuliambut
9416058	9416	Hereapini
9416059	9416	Ubalihi
9416061	9416	Talambo
9416062	9416	Pronggoli
9416063	9416	Endomen
9416065	9416	Kona
9416066	9416	Duram
9416067	9416	Dirwemna
9417010	9417	Iwur
9417011	9417	Kawor
9417012	9417	Tarup
9417013	9417	Awinbon
9417020	9417	Oksibil
9417021	9417	Pepera
9417022	9417	Alemsom
9417023	9417	Serambakon
9417024	9417	Kolomdol
9417025	9417	Oksop
9417026	9417	Ok Bape
9417027	9417	Ok Aon
9417030	9417	Borme
9417031	9417	Bime
9417032	9417	Epumek
9417033	9417	Weime
9417034	9417	Pamek
9417035	9417	Nongme
9417036	9417	Batani
9417040	9417	Okbi
9417041	9417	Aboy
9417042	9417	Okbab
9417043	9417	Teiraplu
9417044	9417	Yefta
9417050	9417	Kiwirok
9417051	9417	Kiwirok Timur
9417052	9417	Oksebang
9417053	9417	Okhika
9417054	9417	Oklip
9417055	9417	Oksamol
9417056	9417	Okbemta
9417060	9417	Batom
9417061	9417	Murkim
9417062	9417	Mofinop
9418010	9418	Kanggime
9418011	9418	Woniki
9418012	9418	Nabunage
9418013	9418	Gilubandu
9418014	9418	Wakuo
9418015	9418	Aweku
9418016	9418	Bogonuk
9418020	9418	Karubaga
9418021	9418	Goyage
9418022	9418	Wunin
9418023	9418	Kondaga
9418024	9418	Nelawi
9418025	9418	Kuari
9418026	9418	Lianogoma
9418027	9418	Biuk
9418030	9418	Bokondini
9418031	9418	Bokoneri
9418032	9418	Bewani
9418040	9418	Kembu
9418041	9418	Wina
9418042	9418	Umagi
9418043	9418	Panaga
9418044	9418	Poganeri
9418045	9418	Kamboneri
9418046	9418	Air Garam
9418047	9418	Dow
9418048	9418	Wari / Taiyeve
9418049	9418	Egiam
9418051	9418	Nunggawi
9418060	9418	Kubu
9418061	9418	Anawi
9418062	9418	Wugi
9418070	9418	Geya
9418071	9418	Wenam
9418080	9418	Numba
9418081	9418	Kai
9418090	9418	Dundu
9418100	9418	Gundagi
9418110	9418	Timori
9418121	9418	Yuneri
9418125	9418	Tagime
9418126	9418	Danime
9418127	9418	Yuko
9418541	9418	Telenggeme
9418542	9418	Gika
9418543	9418	Tagineri
9419021	9419	Pantai Timur Bagian Barat
9419022	9419	Pantai Timur
9419024	9419	Sungai Biri
9419025	9419	Veen
9419031	9419	Bonggo
9419032	9419	Bonggo Timur
9419033	9419	Bonggo Barat
9419040	9419	Tor Atas
9419041	9419	Ismari
9419050	9419	Sarmi
9419051	9419	Sarmi Timur
9419052	9419	Sarmi Selatan
9419053	9419	Sobey
9419054	9419	Muara Tor
9419055	9419	Verkam
9419060	9419	Pantai Barat
9419061	9419	Apawer Hulu
9419062	9419	Apawer Hilir
9419063	9419	Apawer Tengah
9420010	9420	Web
9420011	9420	Towe
9420012	9420	Yaffi
9420020	9420	Senggi
9420021	9420	Kaisenar
9420030	9420	Waris
9420040	9420	Arso
9420041	9420	Arso Timur
9420042	9420	Arso Barat
9420043	9420	Mannem
9420050	9420	Skanto
9426010	9426	Waropen Bawah
9426011	9426	Inggerus
9426012	9426	Urei Faisei
9426013	9426	Oudate
9426014	9426	Wapoga
9426020	9426	Masirei
9426021	9426	Risei Sayati
9426022	9426	Demba
9426023	9426	Soyoi Mambai
9426024	9426	Wonti
9426030	9426	Walani
9426040	9426	Kirihi
9427010	9427	Supiori Selatan
9427011	9427	Kepulauan Aruri
9427020	9427	Supiori Utara
9427021	9427	Supiori Barat
9427030	9427	Supiori Timur
9428030	9428	Waropen Atas
9428031	9428	Benuki
9428032	9428	Sawai
9428040	9428	Mamberamo Ilir
9428050	9428	Mamberamo Tengah
9428051	9428	Iwaso
9428060	9428	Mamberamo Tengah Timur
9428070	9428	Rofaer
9428080	9428	Mamberamo Ulu
9429010	9429	Wosak
9429011	9429	Moba
9429012	9429	Pija
9429013	9429	Kora
9429020	9429	Kenyam
9429021	9429	Mbuwa Tengah
9429022	9429	Krepkuri
9429023	9429	Embetpem
9429030	9429	Geselma
9429031	9429	Kilmid
9429032	9429	Yenggelo
9429033	9429	Alama
9429034	9429	Meborok
9429040	9429	Mapenduma
9429041	9429	Kroptak
9429042	9429	Paro
9429043	9429	Kegayem
9429050	9429	Mugi
9429051	9429	Yal
9429052	9429	Mam
9429060	9429	Yigi
9429061	9429	Dal
9429062	9429	Nirkuri
9429063	9429	Inikgal
9429070	9429	Mbuwa
9429071	9429	Iniye
9429072	9429	Wutpaga
9429073	9429	Nenggeangin
9429074	9429	Mbulmu Yalma
9429080	9429	Gearek
9429081	9429	Pasir Putih
9429082	9429	Wusi
9430010	9430	Makki
9430011	9430	Gupura
9430012	9430	Kolawa
9430013	9430	Gelok Beam
9430014	9430	Awina
9430020	9430	Pirime
9430021	9430	Buguk Gona
9430022	9430	Milimbo
9430023	9430	Gollo
9430024	9430	Wiringgabut
9430030	9430	Tiom
9430031	9430	Nogi
9430032	9430	Mokoni
9430033	9430	Niname
9430034	9430	Yiginua
9430035	9430	Tiom Ollo
9430036	9430	Yugunwi
9430037	9430	Lannyna
9430040	9430	Balingga
9430041	9430	Balingga Barat
9430042	9430	Bruwa
9430043	9430	Ayumnati
9430050	9430	Kuyawage
9430051	9430	Wano Barat
9430060	9430	Malagaineri
9430061	9430	Melagai
9430070	9430	Tiomneri
9430071	9430	Wereka
9430080	9430	Dimba
9430081	9430	Kelulome
9430082	9430	Nikogwe
9430090	9430	Gamelia
9430091	9430	Karu
9430092	9430	Yiluk
9430093	9430	Guna
9430100	9430	Poga
9430101	9430	Muara
9431010	9431	Kobakma
9431020	9431	Ilugwa
9431030	9431	Kelila
9431040	9431	Eragayam
9431050	9431	Megambilis
9432010	9432	Welarek
9432020	9432	Apalapsili
9432030	9432	Abenaho
9432040	9432	Elelim
9432050	9432	Benawa
9433010	9433	Agadugume
9433011	9433	Lambewi
9433012	9433	Oneri
9433020	9433	Gome
9433021	9433	Amungkalpia
9433022	9433	Gome Utara
9433023	9433	Erelmakawia
9433030	9433	Ilaga
9433031	9433	Ilaga Utara
9433032	9433	Mabugi
9433033	9433	Omukia
9433040	9433	Sinak
9433041	9433	Sinak Barat
9433050	9433	Pogoma
9433051	9433	Kembru
9433052	9433	Bina
9433060	9433	Wangbe
9433061	9433	Ogamanim
9433070	9433	Beoga
9433071	9433	Beoga Barat
9433072	9433	Beoga Timur
9433080	9433	Doufo
9433081	9433	Dervos
9434010	9434	Sukikai Selatan
9434020	9434	Piyaiye
9434030	9434	Mapia Barat
9434040	9434	Mapia Tengah
9434050	9434	Mapia
9434060	9434	Dogiyai
9434070	9434	Kamu Selatan
9434080	9434	Kamu
9434090	9434	Kamu Timur
9434100	9434	Kamu Utara
9435010	9435	Homeyo
9435020	9435	Sugapa
9435030	9435	Hitadipa
9435040	9435	Agisiga
9435050	9435	Biandoga
9435060	9435	Wandai
9436010	9436	Kapiraya
9436020	9436	Tigi Barat
9436030	9436	Tigi
9436040	9436	Tigi Timur
9436050	9436	Bowobado
9471010	9471	Muara Tami
9471020	9471	Abepura
9471021	9471	Heram
9471030	9471	Jayapura Selatan
9471040	9471	Jayapura Utara
\.


--
-- PostgreSQL database dump complete
--





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
ADD COLUMN discount t_quantity,
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



CREATE DOMAIN t_taxname AS character varying(100);


ALTER DOMAIN  t_taxname  OWNER TO postgres;



CREATE TABLE m_tax (
    tax_id t_guid PRIMARY KEY,
    name_tax t_taxname,
    tax_percentage t_price,
    description t_description
);

INSERT INTO m_menu (menu_id, name_menu, menu_title, parent_id, order_number, is_active, name_form, is_enabled)
VALUES ('5e936bd0-f01b-44aa-a915-f059883f1189', 'mnuTax', 'Tax', '554bcfc1-cc54-4d49-9282-dde346a57caf', '5', true, 'FrmEntryTax', true);




COPY public.m_role_privilege (role_id, menu_id, grant_id, is_grant) FROM stdin;
11dc1faf-2c66-4525-932d-a90e24da8987	5e936bd0-f01b-44aa-a915-f059883f1189	0	t
11dc1faf-2c66-4525-932d-a90e24da8987	5e936bd0-f01b-44aa-a915-f059883f1189	1	t
11dc1faf-2c66-4525-932d-a90e24da8987	5e936bd0-f01b-44aa-a915-f059883f1189	2	t
11dc1faf-2c66-4525-932d-a90e24da8987	5e936bd0-f01b-44aa-a915-f059883f1189	3	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	5e936bd0-f01b-44aa-a915-f059883f1189	0	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	5e936bd0-f01b-44aa-a915-f059883f1189	1	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	5e936bd0-f01b-44aa-a915-f059883f1189	2	t
c58ee40a-5ae2-4067-b6ad-8cae9c65913c	5e936bd0-f01b-44aa-a915-f059883f1189	3	t
42d0a7b9-2b1a-4ad7-b6ad-b7c8b65ef04a	5e936bd0-f01b-44aa-a915-f059883f1189	0	f
\.



--COPY public.m_item_menu (item_menu_id, menu_id, grant_id, description) FROM stdin;
--fb063166-8527-4a30-b22e-69b3b97b17c8	5e936bd0-f01b-44aa-a915-f059883f1189	0	Melihat Data
--6c5872a2-8ba5-4842-952a-3aa7f7fdae31	5e936bd0-f01b-44aa-a915-f059883f1189	1	Tambah Data
--7931d986-3f1f-4e23-9a11-4ffb7e122067	5e936bd0-f01b-44aa-a915-f059883f1189	2	Edit Data
--7633ad97-ce8d-468b-9b4d-af19d4bcf138	5e936bd0-f01b-44aa-a915-f059883f1189	3	Hapus Data
--\.







ALTER TABLE m_tax ADD COLUMN is_default_tax BOOLEAN DEFAULT FALSE;
UPDATE m_tax SET is_default_tax = TRUE WHERE tax_id = '84b41c6f-6152-4e36-9e45-aa9971a768cb';


CREATE OR REPLACE FUNCTION enforce_single_default_tax()
RETURNS TRIGGER AS $$
BEGIN
  IF NEW.is_default_tax THEN
    UPDATE m_tax SET is_default_tax = FALSE WHERE is_default_tax = TRUE AND tax_id <> NEW.tax_id;
  END IF;
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;


CREATE TRIGGER enforce_single_default_tax_trigger
BEFORE INSERT OR UPDATE ON m_tax
FOR EACH ROW
EXECUTE FUNCTION enforce_single_default_tax();



ALTER TABLE t_sales_order_item
ADD COLUMN tax_id t_guid;


ALTER TABLE t_sales_order_item
ADD CONSTRAINT fk_sales_order_item_tax
FOREIGN KEY (tax_id)
REFERENCES m_tax (tax_id);


ALTER TABLE t_purchase_order_item
ADD COLUMN tax_id t_guid;


ALTER TABLE t_purchase_order_item
ADD CONSTRAINT fk_purchase_order_item_tax
FOREIGN KEY (tax_id)
REFERENCES m_tax (tax_id);


CREATE TABLE TrialInfo (
    Id SERIAL PRIMARY KEY,
    installed_date DATE NOT NULL,
    trial_period INTEGER NOT NULL,
    is_trial BOOLEAN NOT NULL
);

INSERT INTO TrialInfo (installed_date, trial_period, is_trial)
VALUES (CURRENT_DATE, 15, true);