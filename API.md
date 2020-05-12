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
* Returns:
	* `items`: an array containing objects. Each object has an `id` (int), `name` (string), `price` (int), `category` (string) and `image` (string, base64 coded image).

### Item details
Returns a list of items based on a search term.
* Request type: `GET`
* Endpoint: `/item`
* Parameters:
	* `id`: a number containing the id of the item.
* Returns:
	* `item`: an object that has an `id` (int), `name` (string), `price` (int), `category` (string), `image` (string, base64 coded image), `description` (string), `end_date` (string), `seller` (string).

### Category list
Returns a list of available categories.
* Request type: `GET`
* Endpoint: `/categories`
* Returns:
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

### Upload item
Is used to upload a new item to the marketplace
* Request type: `POST`
* Endpoint: `/upload`
* Parameters:
	* `token`: a string containing the session token
	* `name`: a string containing the item name
	* `description`: a string containing a description of the item
	* `image`: a string containing a base64 encoded image of the item
	* `category_id`: an integer containing the category id
	* `start_price`: an integer containing the starting price of the item
	* `buy_price` (optional): an integer containing the quick buy price of the item
* Returns:
	* `success`: a boolean containing if the login was successful

### Favorites
Returns a list of items tagged as favorites by the user.
* Request type: `GET`
* Endpoint: `/favorites`
* Parameters:
	* `token`: a string containing the session token in case of a successful login.
* Returns:
	* `items`: an array containing objects. Each object has an `id` (int), `name` (string), `price` (int), `category` (string) and `image` (string, base64 coded image).

### Notifications
Returns a list of items tagged as favorites by the user.
* Request type: `GET`
* Endpoint: `/notifications`
* Parameters:
	* `token`: a string containing the session token in case of a successful login.
* Returns:
	* `notifications`: an array containing objects. Each object has an `item_id` (int), `item_name` (string), `time` (string) and `type` (int, 0 = ...).