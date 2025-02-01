# FoodQualityWebApp

Application consists of two microservices which communicates together using messaging broker RabbitMQ.
Both microservices has it's own code structure, database and models. They did not know about each other.
They only know how to communicate with each other and every of them can work independently, which is 
one of best caractheristics when it comes to microservice architecture. In order for this to be real 
microservice architecture, there should be separate codebase for both and separate deployment.

I choose RabbitMQ cause it's more suitable option for smaller apps that are running out of devops ecosystem.
Also, RabbitMQ is much easier to start on-premise and local machines and we can configure it easy for our needs.

For choosing SQL Server instead PostgresSQL, there is no particular reason. I'm familiar with both. Both are SQL
databases and will serve the same purpose for this app. We will have migrations, tables, columns and relations 
between using both SQL tools.
