# Vega Systems Office API Configuration Matrix

# Purpose

This matrix documents how the Office Production API should be configured in Development, Testing, and Production environments.

Configuration values may come from:

- appsettings.json
- Environment-specific JSON files
- Environment variables
- .NET user secrets
- An approved production secret store

Sensitive values such as passwords, access tokens, API keys, signing keys, and production database credentials must not be committed to GitHub.

# Configuration Matrix

Configuration Setting		Development	      		Testing					Production							Recommended Source	       			Sensitive?

Environment	                		Development	      		Testing					Production							ASPNETCORE_ENVIRONMENT	           			No
Office Code	                  		VEGA-DEV	    		VEGA-TEST				VEGA-SYSTEMS							JSON or environment variable				No
Office Name	            		Development Office	 		Testing Office				Head Office							JSON or environment variable				No
Report Retention Days	     	 30	                          	 	15                           	    			90	                 							JSON or environment variable				No
Maximum Report Title Length		100				100		   			100								JSON						No
Database Connection		Data Source=Data/office-dev.db	Data Source=Data/office-test.db		Production database placeholder					User secrets or environment variable			Yes in production

Role Header Name			X-Demo-Role			X-Demo-Role				Not applicable after replacing mock authentication				JSON for classroom use				No
User Header Name			X-Demo-User			X-Demo-User				Not applicable after replacing mock authentication				JSON for classroom use				No
Authentication Method		Mock header authentication		Mock header authentication			Microsoft Entra ID, JWT, OpenID Connect, or another trusted provider		Identity-provider configuration				Yes
Default Log Level			Debug	 			Information				Information or Warning						Environment-specific JSON				No
ASP.NET Core Log Level		Information			Warning					Warning								Environment-specific JSON				No
Detailed Errors			Enabled				Disabled					Disabled								Environment-specific setting				No
HTTPS				Development certificate or local HTTP	Test certificate or controlled environment	Valid trusted HTTPS certificate required					Web server or hosting configuration			Certificate is sensitive
Health Endpoint			Enabled				Enabled					Enabled with controlled monitoring access				Application configuration				No
Local SQLite Database		Allowed				Allowed for classroom testing			Not recommended for multi-user production use				Environment-specific configuration		Database content may be sensitive
Mock Error Endpoint			Enabled for demonstration		Enabled only for controlled tests		Disabled								Environment check in code				No
Allowed Hosts			Localhost or * for classroom testing	Testing hostname				Approved production hostname					Environment-specific JSON				No

# Development Environment

The Development environment supports local coding, debugging, and classroom demonstrations.

Recommended characteristics:

- Uses a local development database.
- Uses detailed logging.
- May use the classroom mock identity strategy.
- May expose the intentional error endpoint.
- May use the local ASP.NET Core development certificate.
- Must not contain real production credentials.

Example environment name:

set ASPNETCORE_ENVIRONMENT=Development