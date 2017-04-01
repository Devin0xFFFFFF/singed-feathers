# MapService

## APIs

* CreateMap
* GetMaps
* GetMapData

## Testing

* Make sure you have all requirements.txt packages installed: pip install -r requirements.txt
* In the MapService directory run: PYTHONPATH=service py.test -v

## Deployment

* For each API, zip up a deployment package: zip ${api_snake_case}.zip ${api_snake_case}.py lobby_service_common.py
* Execute an UpdateFunctionCode call to AWS for each API deployment package:
** https://aws.amazon.com/blogs/compute/new-deployment-options-for-aws-lambda/
** http://docs.aws.amazon.com/lambda/latest/dg/API_UpdateFunctionCode.html