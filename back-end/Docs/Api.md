# Runner Car Rental API

- [RUNNERS CAR RENTAL API]
    -[Auth](#auth)
    --[Register](#register)
     --- [Register Request](#register-request)
     --- [Register Response](#register-response)
    --[Login](#login)
    --- [Login Request](#login-request)
    --- [Login Response](#Login-response)


# Auth
### Register 

```js
POST{{host}}/auth/register
```
#### Register Request

```json
{
    "fullName": "Jane Doe",
    "email": "jane@demo.com",
    "password": "Secure123",
    "driverLicenseNumber": "AB123456"
}
```

#### Register Response
```json
{
    "id":"d5as87-89a8s-as9das-asd",
    "fllName": "Jane Doe",
    "eamil":"jane@demo.com",
    "driverLicenseNumber":"AB123456",
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
    "fulltName": " fullName",
    "email": "Secure123",
    "driverLicenseNumber": "driverLicenseNumber",
    "token": "token"
}
```