OpenRetail Database Structure
========================================

OpenRetail uses [PostgreSQL](https://www.postgresql.org/) database version 9.3.16, it is recommended that you use the same version to minimize compatibility issues.

Import OpenRetail Database
-----------------------------------------------
To import OpenRetail database you can use the tool [psql](https://www.postgresql.org/docs/9.2/static/app-psql.html) with the following command:

```
psql -U USERNAME DbOpenRetail < DbOpenRetail.sql
```

For `USERNAME`, you can use the default user [PostgreSQL](https://www.postgresql.org/) namely `postgres`, and make sure that before running the above command the `DbOpenRetail` database has been created first.

Initial Data Import (prerequisites)
-----------------------------------------------
Just like importing the OpenRetail database, we can also use the [psql] tool(https://www.postgresql.org/docs/9.2/static/app-psql.html) with the following command:

```
psql -U postgres DbOpenRetail < 01_data-menu.sql
psql -U postgres DbOpenRetail < 02_data-item_menu.sql
psql -U postgres DbOpenRetail < 03_data-role.sql
psql -U postgres DbOpenRetail < 04_data-role_privilege.sql
psql -U postgres DbOpenRetail < 05_data-pengguna.sql
psql -U postgres DbOpenRetail < 06_data-alasan_penyesuaian_stok.sql
psql -U postgres DbOpenRetail < 07_data-database_version.sql
psql -U postgres DbOpenRetail < 08_data-jenis_pengeluaran.sql
psql -U postgres DbOpenRetail < 09_data-jabatan.sql
psql -U postgres DbOpenRetail < 10_data-profil.sql
psql -U postgres DbOpenRetail < 11_data-header-nota.sql
psql -U postgres DbOpenRetail < 12_data-label-nota.sql
psql -U postgres DbOpenRetail < 13_data-provinsi.sql
psql -U postgres DbOpenRetail < 14_data-kabupaten.sql
psql -U postgres DbOpenRetail < 15_data-header_nota_mini_pos.sql
psql -U postgres DbOpenRetail < 16_data-footer_nota_mini_pos.sql
psql -U postgres DbOpenRetail < 17_data-kartu.sql
psql -U postgres DbOpenRetail < 18_data-setting_aplikasi.sql
psql -U postgres DbOpenRetail < 19_data-provinsi2.sql
psql -U postgres DbOpenRetail < 20_data-kabupaten2.sql
psql -U postgres DbOpenRetail < 21_data-kecamatan.sql
```

Selain cara manual untuk melakukan import struktur database, Anda juga bisa menggunakan file instalasi untuk database development (OpenRetailDatabase-vx.x.x-dev-Setup.exe) yang bisa Anda download di https://github.com/rudi-krsoftware/open-retail/releases.