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

