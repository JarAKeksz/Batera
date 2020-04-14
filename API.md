# API
Our API is a more or less RESTful API using JSON over HTTP.
Here is the list of endpoints and their usage:

### Ping
Answers with a simple message. It can be used to test connection to the server.
* Request type: `GET`
* Endpoint: `/ping`

### Search
Returns a list of items based on a search term.
* Request type: `GET`
* Endpoint: `/search`
* Parameters:
	* `term`: a string containing the serch term
* Retruns:
	* `items`: an array containing objects. Each object has an `id` (int), `name` (string), `price` (int), `category` (string) and `image` (string, base64 coded image).

### Login
Is used to get a session token for a session of a registered user.
* Request type: `POST`
* Endpoint: `/login`
* Parameters:
	* `email`: a string containing the e-mail address
	* `password`: a string containing the password
* Returns:
	* `token`: a string containing the session token