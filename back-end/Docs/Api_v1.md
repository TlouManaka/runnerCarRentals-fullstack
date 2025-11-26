# Runner Car Rental API

- [RUNNERS CAR RENTAL API]
    -[Auth](#auth)
    --[Register](#register)
     --- [Register Request](#register-request)
     --- [Register Response](#register-response)
    --[Login](#login)
    --- [Login Request](#login-request)
    --- [Login Response](#login-response)
    -[Customer](#customer)
    --[Create Profile](#create-profile)
    --- [Create Profile Request](#create-profile-response)
    --- [Ccreate Profile Response](#create-profile-response)
    --[Update Profile](#update-profile)
    --- [Update Profile Request](#update-profile-request)
    --- [Update Profile Response](#update-profile-response)
    --[Delete Profile](#delete-profile)
    --- [Delete Profile Response](#delete-profile-response)


# Auth
### Register 

```js
POST{{host}}/auth/register
```
#### Register Request

```json
{
    "email": "jane@demo.com",
    "password": "Secure123"
}
```

#### Register Response
```json
{
    "eamil":"jane@demo.com",
    "token":"eyjh..akjkjkdsad"
}
```
### Login

```js
POST{{host}}/auth/login
```

#### Login Request

```json
{
    "email": "jane@demo.com",
    "password": "Secure123",
}
```
#### Login Response

```json
{
    "id": "3de9b18d-7c32-490d-ac79-97e6b4c54d81",
    "email": "Secure123",
    "token": "token"
}
```

# Customer 
```js
 POST{{host}}/customer/profile
```
#### Create Profile Request


#### Create Profile Response