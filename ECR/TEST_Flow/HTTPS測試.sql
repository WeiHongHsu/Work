DECLARE 
@ResponseText NVARCHAR(4000)
,@status int
 exec [dbo].[_sp_BartenderAPI] 
 'POST',
 'https://bartender4/Integration/Json_STD_TEST/Execute',
 'application/json'
 ,'[  {  "wh_id": "PZ1",  "carton_number": "202304060129501",  "carton_label": "202304060129501",  "waybill_number": "904046094771",  "waybill_number_barcode": "*904046094771*",  "order_number": "202304060129501",  "order_number_barcode": "*202304060129501*",  "display_order_number": "202304060129501",  "ship_to_name": "�����a",  "ship_to_address": "�s�˥��F�ϩ����1229��1��",  "ship_to_phone": "8860972133180",  "ship_date": "2023-06-05",  "delivery_date": "2023-06-06",  "cod_flag": "������",  "hope_delivery_date": "6��6��",  "zip_7": "30-293-03-E",  "zip_7_2": "3029303",  "db_version": "23060301",  "qr_code": "01|904046094771|10|279529660100|N|0|01|01|02|29303E|20230606|01||0|||||||||||",  "ship_to_address2": "*******��1229��1��",  "printer_id": "A4_192.168.19.81",  "FilePath_bak": "D:\\Bartender\\LogFiles\\TCAT_ShippingLabel_202304060129501_20230605093059.Json",  "BTW_Label": "D:\\Bartender\\Format\\TCAT_ShippingLabel.btw"  }  ]'
 ,'JackyHsu'
 , 'LNWOC210009'
 ,'TEST123'
 , 1
 , @status output
 ,@ResponseText output
 select @status,@ResponseText
