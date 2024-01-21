#1 Activity3PService

The following project simulates an Electricity Utility Provider provider service. The service exposes a simple REST API to authenticate and retrieve fake consumption data.
The service data shape is designed to fit the model of Microsoft Sustainability Manager (MSM) data model.
It is designed to serve as a test and an example service for documentation of development of a 3rd party partner connectors to MSM.

In summary the service authenticate username/password and returns a Bearer token to be set in Authentication header in all consequent calls
Upon request, the server returns fake purchased electricity records, one per day since a watermark date marker.


Deploy the service on a REST compatible ASP Core web server.

## API Usage

### Authenticate

#### Call: POST Logon/Post

Request Body should contain:
```
{
    "username": "***removed***",
    "password":  "***removed***"
}
Return: Authentication Token (SHA256 based).

#### Call: GET Activity/GetActivities

Authentication: Bearer <token>
Parameters:
- ActivityType = Purchased Electricity  // hard-coded, only supported type.
- Facility = The facility you wish to associate the PE record
- OU = similarly for OU
- WatermarkDate = The response will contain a record per day since WatermarkDate
- NameSeed = The record names will start with NameSeed string




## Swagger
```

{
    "openapi": "3.0.1",
    "info": {
        "title": "Activity3PService",
        "version": "1.0"
    },
    "paths": {
        "/Activity/GetActivities": {
            "get": {
                "tags": [
                    "Activity"
                ],
                "operationId": "GetActivities",
                "parameters": [
                    {
                        "name": "ActivityType",
                        "in": "query",
                        "schema": {
                            "type": "string"
                        }
                    },
                    {
                        "name": "Facility",
                        "in": "query",
                        "schema": {
                            "type": "string"
                        }
                    },
                    {
                        "name": "OU",
                        "in": "query",
                        "schema": {
                            "type": "string"
                        }
                    },
                    {
                        "name": "WatermarkDate",
                        "in": "query",
                        "schema": {
                            "type": "string"
                        }
                    },
                    {
                        "name": "NameSeed",
                        "in": "query",
                        "schema": {
                            "type": "string",
                            "default": "Test 3P Utilities"
                        }
                    }
                ],
                "responses": {
                    "200": {
                        "description": "Success",
                        "content": {
                            "text/plain": {
                                "schema": {
                                    "$ref": "#/components/schemas/ActivityResponseModel"
                                }
                            },
                            "application/json": {
                                "schema": {
                                    "$ref": "#/components/schemas/ActivityResponseModel"
                                }
                            },
                            "text/json": {
                                "schema": {
                                    "$ref": "#/components/schemas/ActivityResponseModel"
                                }
                            }
                        }
                    }
                }
            }
        },
        "/Logon/Post": {
            "post": {
                "tags": [
                    "Logon"
                ],
                "requestBody": {
                    "content": {
                        "application/json": {
                            "schema": {
                                "$ref": "#/components/schemas/LogonModel"
                            }
                        },
                        "text/json": {
                            "schema": {
                                "$ref": "#/components/schemas/LogonModel"
                            }
                        },
                        "application/*+json": {
                            "schema": {
                                "$ref": "#/components/schemas/LogonModel"
                            }
                        }
                    }
                },
                "responses": {
                    "200": {
                        "description": "Success"
                    }
                }
            }
        },
        "/Logon/Verify": {
            "get": {
                "tags": [
                    "Logon"
                ],
                "operationId": "Verify",
                "parameters": [
                    {
                        "name": "token",
                        "in": "query",
                        "schema": {
                            "type": "string"
                        }
                    }
                ],
                "responses": {
                    "200": {
                        "description": "Success"
                    }
                }
            }
        }
    },
    "components": {
        "schemas": {
            "ActivityModel": {
                "type": "object",
                "properties": {
                    "name": {
                        "type": "string",
                        "nullable": true
                    },
                    "description": {
                        "type": "string",
                        "nullable": true
                    },
                    "quantity": {
                        "type": "string",
                        "nullable": true
                    },
                    "quantityUnit": {
                        "type": "string",
                        "nullable": true
                    },
                    "dataQualityType": {
                        "type": "string",
                        "nullable": true
                    },
                    "energyProviderName": {
                        "type": "string",
                        "nullable": true
                    },
                    "contractualInstrumentType": {
                        "type": "string",
                        "nullable": true
                    },
                    "isRenewable": {
                        "type": "string",
                        "nullable": true
                    },
                    "organizationalUnit": {
                        "type": "string",
                        "nullable": true
                    },
                    "facility": {
                        "type": "string",
                        "nullable": true
                    },
                    "transactionDate": {
                        "type": "string",
                        "nullable": true
                    },
                    "consumptionStartDate": {
                        "type": "string",
                        "nullable": true
                    },
                    "consumptionEndDate": {
                        "type": "string",
                        "nullable": true
                    },
                    "evidence": {
                        "type": "string",
                        "nullable": true
                    },
                    "originCorrelationID": {
                        "type": "string",
                        "nullable": true
                    }
                },
                "additionalProperties": false
            },
            "ActivityResponseModel": {
                "type": "object",
                "properties": {
                    "statusCode": {
                        "type": "integer",
                        "format": "int32"
                    },
                    "message": {
                        "type": "string",
                        "nullable": true
                    },
                    "count": {
                        "type": "integer",
                        "format": "int32"
                    },
                    "activities": {
                        "type": "array",
                        "items": {
                            "$ref": "#/components/schemas/ActivityModel"
                        },
                        "nullable": true
                    }
                },
                "additionalProperties": false
            },
            "LogonModel": {
                "type": "object",
                "properties": {
                    "username": {
                        "type": "string",
                        "nullable": true
                    },
                    "password": {
                        "type": "string",
                        "nullable": true
                    }
                },
                "additionalProperties": false
            }
        }
    }
}
```