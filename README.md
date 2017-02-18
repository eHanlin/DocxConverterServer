# DocxConverterServer
simpe server to convert docx to pdf or doc

api:
<br>
{host}/api/Converter/ConvertToPdf ->convert docx to pdf
<br>
{host}/api/Converter/ConvertToDoc ->convert docx to doc

required:
office,rabbitmq,mongo,aws s3 storge

<hr>
使用前須先將web.conig中的aws s3,mongo ip,rabbitmq ip填入
<hr>
作業流程：
webapi接收資訊->下載word至app_data\words->轉檔->上傳s3->將轉檔後的下載連結送到callback