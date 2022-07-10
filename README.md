# **Chatto**
##### _Simple texting web application as a monolith supported by microservices_
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
## Presentation
To present how Chatto is performing `Chatto.postman_collection.json` file was added to repository.
#### Notes
* To use any endpoint that requires authentication JWT token should be added to header 
`{"Authorization": "Bearer <JWT token>" }`
* To check to whom JWT token belongs please use endpoint `/api/Authentication/WhoAmI`. 
If token is invalid `401 Unauthorized` will be returned.
* Using `/api/Authentication/LoginChatto"` is a specific case in which login data should be send not in body, but in header like this:
`{"Authorization": "<Username>.<Password>"}`

## Front-End
Project also contains not finished front-end application made in React using typescript. The purpose is to show how google authentication cold look like and work on client's side.
