USE [DBSoChiTieu]
GO

DECLARE @CreatedBy UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000001'

INSERT INTO [dbo].[ct_Currency]
([Id],[Code],[Name],[Symbol],[CreatedDate],[CreatedBy],[ModifiedDate],[ModifiedBy])
VALUES
(NEWID(),'USD','US Dollar','$',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'EUR','Euro','€',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'JPY','Japanese Yen','¥',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'GBP','British Pound','£',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'AUD','Australian Dollar','$',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'CAD','Canadian Dollar','$',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'CHF','Swiss Franc','CHF',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'CNY','Chinese Yuan','¥',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'HKD','Hong Kong Dollar','$',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'NZD','New Zealand Dollar','$',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'SEK','Swedish Krona','kr',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'KRW','South Korean Won','₩',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'SGD','Singapore Dollar','$',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'NOK','Norwegian Krone','kr',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'MXN','Mexican Peso','$',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'INR','Indian Rupee','₹',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'RUB','Russian Ruble','₽',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'ZAR','South African Rand','R',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'TRY','Turkish Lira','₺',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'BRL','Brazilian Real','R$',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'TWD','New Taiwan Dollar','$',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'DKK','Danish Krone','kr',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'PLN','Polish Zloty','zł',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'THB','Thai Baht','฿',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'IDR','Indonesian Rupiah','Rp',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'HUF','Hungarian Forint','Ft',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'CZK','Czech Koruna','Kč',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'ILS','Israeli Shekel','₪',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'CLP','Chilean Peso','$',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'PHP','Philippine Peso','₱',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'AED','UAE Dirham','د.إ',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'COP','Colombian Peso','$',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'SAR','Saudi Riyal','﷼',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'MYR','Malaysian Ringgit','RM',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'RON','Romanian Leu','lei',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'ARS','Argentine Peso','$',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'PKR','Pakistani Rupee','₨',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'VND','Vietnamese Dong','₫',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'BDT','Bangladeshi Taka','৳',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'EGP','Egyptian Pound','£',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'LKR','Sri Lankan Rupee','₨',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'NGN','Nigerian Naira','₦',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'KZT','Kazakhstani Tenge','₸',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'QAR','Qatari Riyal','﷼',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'KWD','Kuwaiti Dinar','د.ك',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'OMR','Omani Rial','﷼',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'BHD','Bahraini Dinar','BD',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'JOD','Jordanian Dinar','JD',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'MAD','Moroccan Dirham','د.م.',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'TND','Tunisian Dinar','د.ت',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'UGX','Ugandan Shilling','USh',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'KES','Kenyan Shilling','KSh',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'GHS','Ghanaian Cedi','₵',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'XOF','West African CFA Franc','CFA',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'XAF','Central African CFA Franc','FCFA',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'ETB','Ethiopian Birr','Br',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'DZD','Algerian Dinar','دج',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'UAH','Ukrainian Hryvnia','₴',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'BGN','Bulgarian Lev','лв',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'HRK','Croatian Kuna','kn',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'ISK','Icelandic Krona','kr',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'GEL','Georgian Lari','₾',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'AMD','Armenian Dram','֏',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'AZN','Azerbaijani Manat','₼',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'MNT','Mongolian Tugrik','₮',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'LAK','Lao Kip','₭',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'KHR','Cambodian Riel','៛',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'MMK','Myanmar Kyat','Ks',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'BND','Brunei Dollar','$',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'FJD','Fijian Dollar','$',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'PGK','Papua New Guinea Kina','K',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'SBD','Solomon Islands Dollar','$',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'TOP','Tongan Paʻanga','T$',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'WST','Samoan Tala','WS$',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'XPF','CFP Franc','₣',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'BWP','Botswana Pula','P',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'NAD','Namibian Dollar','$',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'SZL','Swazi Lilangeni','E',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'MUR','Mauritian Rupee','₨',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'SCR','Seychellois Rupee','₨',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'MVR','Maldivian Rufiyaa','Rf',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'BTN','Bhutanese Ngultrum','Nu.',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'NPR','Nepalese Rupee','₨',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'AFN','Afghan Afghani','؋',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'IRR','Iranian Rial','﷼',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'IQD','Iraqi Dinar','ع.د',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'SYP','Syrian Pound','£',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'LBP','Lebanese Pound','ل.ل',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'YER','Yemeni Rial','﷼',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'SDG','Sudanese Pound','£',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'SOS','Somali Shilling','Sh',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'TZS','Tanzanian Shilling','TSh',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'RWF','Rwandan Franc','FRw',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'BIF','Burundian Franc','FBu',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'MWK','Malawian Kwacha','MK',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'ZMW','Zambian Kwacha','ZK',GETDATE(),@CreatedBy,NULL,NULL),
(NEWID(),'AOA','Angolan Kwanza','Kz',GETDATE(),@CreatedBy,NULL,NULL)
GO

UPDATE ct_Currency SET Symbol = N'₫' WHERE Code = 'VND'
UPDATE ct_Currency SET Symbol = N'₹' WHERE Code = 'INR'
UPDATE ct_Currency SET Symbol = N'₨' WHERE Code IN ('PKR','LKR','NPR','MUR','SCR')
UPDATE ct_Currency SET Symbol = N'₩' WHERE Code = 'KRW'
UPDATE ct_Currency SET Symbol = N'₺' WHERE Code = 'TRY'
UPDATE ct_Currency SET Symbol = N'₦' WHERE Code = 'NGN'
UPDATE ct_Currency SET Symbol = N'₸' WHERE Code = 'KZT'
UPDATE ct_Currency SET Symbol = N'₮' WHERE Code = 'MNT'
UPDATE ct_Currency SET Symbol = N'₭' WHERE Code = 'LAK'
UPDATE ct_Currency SET Symbol = N'₱' WHERE Code = 'PHP'
UPDATE ct_Currency SET Symbol = N'₴' WHERE Code = 'UAH'
UPDATE ct_Currency SET Symbol = N'₾' WHERE Code = 'GEL'
UPDATE ct_Currency SET Symbol = N'₼' WHERE Code = 'AZN'
UPDATE ct_Currency SET Symbol = N'֏' WHERE Code = 'AMD'
UPDATE ct_Currency SET Symbol = N'؋' WHERE Code = 'AFN'
UPDATE ct_Currency SET Symbol = N'৳' WHERE Code = 'BDT'
UPDATE ct_Currency SET Symbol = N'﷼' WHERE Code IN ('IRR','SAR','QAR','YER','OMR')
UPDATE ct_Currency SET Symbol = N'د.إ' WHERE Code = 'AED'
UPDATE ct_Currency SET Symbol = N'د.ك' WHERE Code = 'KWD'
UPDATE ct_Currency SET Symbol = N'د.ت' WHERE Code = 'TND'
UPDATE ct_Currency SET Symbol = N'دج' WHERE Code = 'DZD'
UPDATE ct_Currency SET Symbol = N'ل.ل' WHERE Code = 'LBP'
UPDATE ct_Currency SET Symbol = N'ع.د' WHERE Code = 'IQD'
go