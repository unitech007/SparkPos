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
-- Name: t_nota; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_nota AS character varying(20);


ALTER DOMAIN t_nota OWNER TO postgres;

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
    var_pembayaran_hutang_product_id := OLD.pembayaran_hutang_product_id;
	
    var_row_count := (select count(*) from t_debt_payment_item
                      where pembayaran_hutang_product_id = var_pembayaran_hutang_product_id);
                      
    IF var_row_count IS NULL THEN
        var_row_count := 0;  
    END IF;
	
	IF (var_row_count = 0) THEN
    	DELETE FROM t_product_payable_payment WHERE pembayaran_hutang_product_id = var_pembayaran_hutang_product_id;
    END IF;
    
  	RETURN NULL;
END;
$$;


ALTER FUNCTION public.f_delete_payable_product_header() OWNER TO postgres;

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

CREATE FUNCTION f_reduce_product_stock() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
DECLARE 
	var_product_id 				t_guid;
    
	var_stok_sekarang 			t_quantity; -- stok etalase	
    var_warehouse_stock_sekarang 	t_quantity; -- stok gudang
    
    var_quantity_lama 		t_quantity;
	var_quantity_baru 		t_quantity;
    
    var_return_quantity_lama 	t_quantity;
    var_return_quantity_baru 	t_quantity;

	var_price 				t_price;
    
    is_retur				t_bool;
    is_update_iselling_price	t_bool;
    
BEGIN		
	is_retur := FALSE;
                
    IF TG_OP = 'INSERT' THEN
    	var_product_id := NEW.product_id;
        var_quantity_baru = NEW.quantity;        
        var_price = NEW.iselling_price;

	ELSIF TG_OP = 'UPDATE' THEN      
    	var_product_id := NEW.product_id;
        var_quantity_baru = NEW.quantity;
        var_quantity_lama = OLD.quantity;
        var_price = NEW.iselling_price;
        
        -- quantity retur
        var_return_quantity_baru = NEW.return_quantity;
        var_return_quantity_lama = OLD.return_quantity;        
    ELSE
    	var_product_id := OLD.product_id;
        var_quantity_lama = OLD.quantity;
        var_price = OLD.iselling_price;
    END IF;        
            
    SELECT stok, warehouse_stock INTO var_stok_sekarang, var_warehouse_stock_sekarang
    FROM m_product WHERE product_id = var_product_id;
    
    IF var_stok_sekarang IS NULL THEN -- stok etalase
        var_stok_sekarang := 0;  
    END IF;
    
    IF var_warehouse_stock_sekarang IS NULL THEN -- stok gudang
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
            
            IF (var_warehouse_stock_sekarang < 0 AND var_stok_sekarang > 0) THEN -- stok gudang kurang, sisanya ambil dari stok etalase
            	var_stok_sekarang = var_stok_sekarang - abs(var_warehouse_stock_sekarang);
                
                IF (var_stok_sekarang >= 0) THEN
                	var_warehouse_stock_sekarang := 0; --stok gudang habis
                ELSE
                	var_warehouse_stock_sekarang := var_stok_sekarang;
                    var_stok_sekarang := 0;
                END IF;
            END IF;
                        
            
        ELSIF TG_OP = 'UPDATE' THEN      
            var_warehouse_stock_sekarang = var_warehouse_stock_sekarang + var_quantity_lama - var_quantity_baru;
        ELSE
            var_warehouse_stock_sekarang = var_warehouse_stock_sekarang + var_quantity_lama;
        END IF;
        
        IF TG_OP = 'INSERT' THEN   
            -- baca setting aplikasi
        	SELECT is_update_iselling_price_master_produk INTO is_update_iselling_price
            FROM m_application_setting LIMIT 1;
            
            IF is_update_iselling_price IS NULL THEN
				is_update_iselling_price := TRUE;
            END IF;
    
        	IF (is_update_iselling_price = TRUE) THEN -- update price jual di master produk
            	UPDATE m_product SET stok = var_stok_sekarang, warehouse_stock = var_warehouse_stock_sekarang, iselling_price = var_price WHERE product_id = var_product_id;
            ELSE          
	            UPDATE m_product SET stok = var_stok_sekarang, warehouse_stock = var_warehouse_stock_sekarang WHERE product_id = var_product_id;        
			END IF;         
        ELSE        
            UPDATE m_product SET stok = var_stok_sekarang, warehouse_stock = var_warehouse_stock_sekarang WHERE product_id = var_product_id;        
        END IF;    
    END IF;	
            
    RETURN NULL;
END;
$$;


ALTER FUNCTION public.f_reduce_product_stock() OWNER TO postgres;

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
	
    var_stok_awal				t_quantity;
	var_stok_akhir				t_quantity;
    var_stok_sekarang 			t_quantity; -- stok etalase
	var_warehouse_stock_sekarang 	t_quantity; -- stok gudang
    
    var_quantity_lama 			t_quantity;
	var_quantity_baru 			t_quantity;
	
    var_return_quantity_lama 		t_quantity;
    var_return_quantity_baru 		t_quantity;
    
	var_price 					t_price; -- price beli di item beli
    var_hpp_awal				t_price; -- price beli di master produk
    var_hpp_akhir				t_price;
	var_starting_balance				t_price; -- (stok etalasi + stok gudang) * price beli di master produk 
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
    
    SELECT stok, warehouse_stock, purchase_price INTO var_stok_sekarang, var_warehouse_stock_sekarang, var_hpp_awal
    FROM m_product WHERE product_id = var_product_id;
    
    IF var_stok_sekarang IS NULL THEN
    	var_stok_sekarang := 0;  
	END IF;
    
    IF var_warehouse_stock_sekarang IS NULL THEN
    	var_warehouse_stock_sekarang := 0;  
	END IF;
    
    IF var_hpp_awal IS NULL THEN
    	var_hpp_awal := 0;  
	END IF;        
    
    var_stok_akhir := 1;
    var_stok_awal := var_stok_sekarang + var_warehouse_stock_sekarang;    
    var_starting_balance := var_stok_awal * var_hpp_awal;
    
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
            
            var_stok_akhir := (var_stok_awal - ABS(var_return_quantity_lama - var_return_quantity_baru));
            IF (var_stok_akhir = 0) THEN
            	var_stok_akhir := 1;
            END IF;
            
            var_hpp_akhir := (var_starting_balance - var_saldo_pembelian) / var_stok_akhir;
                                       
            var_warehouse_stock_sekarang = var_warehouse_stock_sekarang + var_return_quantity_lama - var_return_quantity_baru;			            
                          			            
            UPDATE m_product SET warehouse_stock = var_warehouse_stock_sekarang, purchase_price = ROUND(var_hpp_akhir, 0) WHERE product_id = var_product_id;
        END IF;        
    END IF;
    
    IF (is_retur = FALSE) THEN -- bukan retur    	        	                                    
      IF TG_OP = 'INSERT' THEN                        
          var_saldo_pembelian := var_quantity_baru * var_price;          
          
          var_stok_akhir := (var_stok_awal + var_quantity_baru);
          
          IF (var_stok_akhir = 0) THEN
              var_stok_akhir := 1;
          END IF;
          
		  var_hpp_akhir := (var_starting_balance + var_saldo_pembelian) / var_stok_akhir;                                         
          
          var_warehouse_stock_sekarang = var_warehouse_stock_sekarang + var_quantity_baru;
          
      ELSIF TG_OP = 'UPDATE' THEN             
          var_saldo_pembelian := ABS(var_quantity_baru - var_quantity_lama) * var_price;
          var_hpp_akhir := var_hpp_awal;
          
      	  IF (var_quantity_baru > var_quantity_lama) THEN      
          	
          	 var_stok_akhir := (var_stok_awal + var_quantity_baru - var_quantity_lama);
             IF (var_stok_akhir = 0) THEN
             	var_stok_akhir := 1;
             END IF;

          	 var_hpp_akhir := (var_starting_balance + var_saldo_pembelian) / var_stok_akhir;                     
             
          ELSEIF (var_quantity_baru < var_quantity_lama) THEN          	 
          	 
          	 var_stok_akhir := (var_stok_awal - ABS(var_quantity_baru - var_quantity_lama));
             IF (var_stok_akhir = 0) THEN
             	var_stok_akhir := 1;
             END IF;
             
          	 var_hpp_akhir := (var_starting_balance - var_saldo_pembelian) / var_stok_akhir;                                   
          END IF;          
                                        
          var_warehouse_stock_sekarang = var_warehouse_stock_sekarang - var_quantity_lama + var_quantity_baru;
      ELSE -- DELETE
          var_saldo_pembelian := var_quantity_lama * var_price;
          
          var_stok_akhir := (var_stok_awal - var_quantity_lama);
          IF (var_stok_akhir = 0) THEN
             var_stok_akhir := 1;
          END IF;
             
          var_hpp_akhir := (var_starting_balance - var_saldo_pembelian) / var_stok_akhir;                     
                    
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
    WHERE item_beli_product_id = var_purchase_item_id;
	
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
	var_item_sale_id 		t_guid;
    var_return_quantity		t_quantity;
        
BEGIN	
      
	IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN
    	var_item_sale_id := NEW.item_sale_id;    
        var_return_quantity := NEW.return_quantity;
    ELSE
    	var_item_sale_id := OLD.item_sale_id;
        var_return_quantity := 0;
    END IF;
    
    IF var_return_quantity IS NULL THEN
        var_return_quantity := 0;  
    END IF; 
            
    UPDATE t_sales_order_item SET return_quantity = var_return_quantity 
    WHERE item_sale_id = var_item_sale_id;
	
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
DECLARE var_beli_product_id t_guid;
DECLARE var_pelunasan_nota t_price;
  
BEGIN
	IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN
    	var_beli_product_id := NEW.beli_product_id;    
    ELSE
    	var_beli_product_id := OLD.beli_product_id;
    END IF;
	        
    var_pelunasan_nota := (SELECT SUM(amount) FROM t_debt_payment_item 
    			 	       WHERE beli_product_id = var_beli_product_id);
	
    IF var_pelunasan_nota IS NULL THEN
    	var_pelunasan_nota := 0;  
	END IF;
    
    UPDATE t_purchase_product SET total_payment = var_pelunasan_nota 
    WHERE beli_product_id = var_beli_product_id;                        
    
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
    
    UPDATE t_product_salesSET total_payment = var_pelunasan_nota 
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
	var_beli_product_id 		t_guid;
    var_retur_beli_id		t_guid;
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
    	var_beli_product_id := NEW.beli_product_id;    
    ELSE
    	var_beli_product_id := OLD.beli_product_id;
    END IF;	 
                       
    var_total_nota := (SELECT SUM((quantity - return_quantity) * (price - (discount / 100 * price))) 
    				   FROM t_purchase_order_item
					   WHERE beli_product_id = var_beli_product_id);
	
    IF var_total_nota IS NULL THEN
    	var_total_nota := 0;  
	END IF;              
    --
    SELECT tax, discount INTO var_ppn, var_diskon
    FROM t_purchase_product WHERE beli_product_id = var_beli_product_id;            
    
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
			
            var_retur_beli_id := (SELECT retur_beli_product_id FROM t_purchase_return WHERE beli_product_id = var_beli_product_id LIMIT 1);
			var_date_tempo := (SELECT date_tempo FROM t_purchase_product WHERE beli_product_id = var_beli_product_id);                                    
            
            IF var_date_tempo IS NULL THEN -- nota tunai            	
                UPDATE t_purchase_product SET total_invoice = ROUND(var_total_nota, 0), retur_beli_product_id = var_retur_beli_id 
                WHERE beli_product_id = var_beli_product_id;                
                
                UPDATE t_debt_payment_item SET amount = ROUND(var_total_nota, 0) - var_diskon + var_ppn
                WHERE beli_product_id = var_beli_product_id;
            ELSE
            	UPDATE t_purchase_product SET total_invoice = ROUND(var_total_nota, 0), retur_beli_product_id = var_retur_beli_id 
                WHERE beli_product_id = var_beli_product_id;
            END IF;        
        END IF;        
    END IF;
    
    IF (is_retur = FALSE) THEN -- bukan retur    	
	    UPDATE t_purchase_product SET total_invoice = ROUND(var_total_nota, 0) 
        WHERE beli_product_id = var_beli_product_id;
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
            WHERE pembayaran_hutang_product_id IN (SELECT pembayaran_hutang_product_id FROM t_debt_payment_item WHERE beli_product_id = NEW.beli_product_id);                
            
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
    var_retur_sale_id		t_guid;
    var_diskon				t_price;
    var_ppn					t_price;
	var_shipping_cost		t_price;
	var_total_nota 			t_price;        
    var_return_quantity_lama 	t_quantity;
    var_return_quantity_baru 	t_quantity;
    var_date_tempo		DATE;
    is_retur				t_bool;
    
BEGIN
	is_retur := FALSE;
    
	IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN
    	var_sale_id := NEW.sale_id;    
    ELSE
    	var_sale_id := OLD.sale_id;        
    END IF;	    	          

    var_total_nota := (SELECT SUM((quantity - return_quantity) * (iselling_price - (discount / 100 * iselling_price))) 
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
			
            var_retur_sale_id := (SELECT retur_sale_id FROM t_sales_return WHERE sale_id = var_sale_id LIMIT 1);
            var_date_tempo := (SELECT date_tempo FROM t_product_sales WHERE sale_id = var_sale_id);                                                
            
            IF var_date_tempo IS NULL THEN -- nota tunai            	
                UPDATE t_product_sales SET total_invoice = ROUND(var_total_nota, 0), retur_sale_id = var_retur_sale_id 
                WHERE sale_id = var_sale_id;                
                
                UPDATE t_credit_payment_item SET amount = ROUND(var_total_nota, 0) - var_diskon + var_ppn + var_shipping_cost
                WHERE sale_id = var_sale_id;
            ELSE
            	UPDATE t_product_sales SET total_invoice = ROUND(var_total_nota, 0), retur_sale_id = var_retur_sale_id 
                WHERE sale_id = var_sale_id;
            END IF;        
        END IF;        
    END IF;              

	IF (is_retur = FALSE) THEN -- bukan retur    	
    	UPDATE t_product_sales SET total_invoice = ROUND(var_total_nota, 0), retur_sale_id = NULL 
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
        
    v_total := (SELECT SUM(quantity * price) FROM t_item_pengeluaran_biaya
			    WHERE expense_id = v_expense_id);
	
    IF v_total IS NULL THEN
    	v_total := 0;  
	END IF;
    
    UPDATE t_expenceSET total = v_total WHERE expense_id = v_expense_id;                        
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
    WHERE date_tempo IS NOT NULL AND customer_id = var_customer_id;
    
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
            WHERE date_tempo IS NOT NULL AND customer_id = var_customer_id_old;
            
            IF var_total_credit IS NULL THEN
                var_total_credit := 0;  
            END IF;
            
            IF var_total_settlement IS NULL THEN
                var_total_settlement := 0;  
            END IF;                           
			
			UPDATE m_customer SET total_credit = var_total_credit, total_receivable_payment = var_total_settlement
            WHERE customer_id = var_customer_id_old;
            
            UPDATE t_product_receivable_paymentSET customer_id = var_customer_id 
            WHERE pay_sale_id IN (SELECT pay_sale_id FROM t_credit_payment_itemWHERE sale_id = NEW.sale_id);
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
	var_retur_beli_product_id	t_guid;
	var_total_nota 		t_price;
    
BEGIN    
    IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN
    	var_retur_beli_product_id := NEW.retur_beli_product_id;    
    ELSE
    	var_retur_beli_product_id := OLD.retur_beli_product_id;
    END IF;
        
    var_total_nota := (SELECT SUM(return_quantity * price) 
    				   FROM t_item_retur_beli_produk
					   WHERE retur_beli_product_id = var_retur_beli_product_id);
                           
    IF var_total_nota IS NULL THEN
    	var_total_nota := 0;  
	END IF;                         
    
    UPDATE t_purchase_return SET total_invoice = var_total_nota 
    WHERE retur_beli_product_id = var_retur_beli_product_id;
    
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
	var_retur_sale_id	t_guid;
	var_total_nota 		t_price;
    
BEGIN    
    IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN
    	var_retur_sale_id := NEW.retur_sale_id;    
    ELSE
    	var_retur_sale_id := OLD.retur_sale_id;
    END IF;
        
    var_total_nota := (SELECT SUM(return_quantity * iselling_price) 
    				   FROM t_item_retur_jual_produk
					   WHERE retur_sale_id = var_retur_sale_id);
                           
    IF var_total_nota IS NULL THEN
    	var_total_nota := 0;  
	END IF;                         
    
    UPDATE t_sales_return SET total_invoice = var_total_nota 
    WHERE retur_sale_id = var_retur_sale_id;
    
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
        var_iselling_price := NEW.iselling_price;        
        var_iselling_price_old := OLD.iselling_price;    	
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
    receivable_credit_limit t_price,
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
    subdistrict_id character(7)
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