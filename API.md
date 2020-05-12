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
	* `Name` (optional): a string containing the serch term for name.
	* `CategoryId` (optional): a number containing the category id.
	* `IsItNew` (optional): a boolean ("true"/"false") containing if item should be new.
	* `BuyWithoutBid` (optional): a boolean ("true"/"false") containing if item can be purchased directly.
	* `MinPrice` (optional): a number containing minimum price (inclusive).
	* `MaxPrice` (optional): a number containing maximum price (inclusive).
* Retruns:
	* `items`: an array containing objects. Each object has an `id` (int), `name` (string), `price` (int), `category` (string) and `image` (string, base64 coded image).

### Item details
Returns a list of items based on a search term.
* Request type: `GET`
* Endpoint: `/item`
* Parameters:
	* `id`: a number containing the id of the item.
* Retruns:
	* `item`: an object that has an `id` (int), `name` (string), `price` (int), `category` (string), `image` (string, base64 coded image), `description` (string), `end_date` (string), `seller` (string).

### Category list
Returns a list of available categories.
* Request type: `GET`
* Endpoint: `/categories`
* Retruns:
	* `categories`: an array containing objects. Each object has an `id` (int) and `name` (string).

### Login
Is used to get a session token for a session of a registered user.
* Request type: `POST`
* Endpoint: `/login`
* Parameters:
	* `email`: a string containing the e-mail address
	* `password`: a string containing the password
* Returns:
	* `success`: a boolean containing if the login was successful
	* `token`: a string containing the session token in case of a successful login

### Favorites
Returns a list of items tagged as favorites by the user.
* Request type: `GET`
* Endpoint: `/favorites`
* Parameters:
	* `id`: a number containing the id of the user.
* Retruns:
	* `items`: an array containing objects. Each object has an `id` (int), `name` (string), `price` (int), `category` (string) and `image` (string, base64 coded image).