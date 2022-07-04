# **Chatto**
##### _Simple texting monolith web application supported by some microservices_
------------------------


## Description
College project with a goal of making funcional API for text application Chatto. The architecture is one monolith supported by two microservices Guid and Authentication. The comminication between them is handled by HttpClients without using external libraries or nuget packages.
## Functionality
#### Authentication
* Create account via google OAuth
* Create account via Login and Password (password of course stored as hash)
##### Notes
* Whole process is computed in separate microservice
* Authentication is handled via JWT token
#### Communication
* Create user with an account
* Create text channel
* Invite other user to text channel
* Decline or Accept text channel invitation
* Send message to text channel
* Read all messages from text chnnel
* Get all text channels that you are a member of
##### Notes
* All those APIs use Authorization and Authentication if needed
* Communication data is stored in the monolith
* All guids come from Guid microservice
#### Guid generation
* Get new guid
##### Notes
* All guids are generated on a separate Guid microservice


## Front-End
Project also contains not finished front-end application made in React using typescript. The purpose is to show how google authentication cold look like and work on client's side.
