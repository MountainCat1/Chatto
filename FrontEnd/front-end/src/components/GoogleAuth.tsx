import { CodeResponse, GoogleLogin, useGoogleLogin } from "@react-oauth/google";

export default function () {
    return (
        <div className="d-flex flex-column justify-content-around">
            <div>
                <h2>Register</h2>
                <GoogleLogin
                    onSuccess={credentialResponse => {
                        callApiRegister(credentialResponse.credential as string);
                    }}
                    onError={() => {
                        console.log('Login Failed');
                    }}
                />
            </div>

            <div>
            <h2>Login</h2>
                <GoogleLogin
                    onSuccess={credentialResponse => {
                        callApiLogin(credentialResponse.credential as string);
                    }}
                    onError={() => {
                        console.log('Login Failed');
                    }}
                />
            </div>
        </div>
    )
}

// Calls Back-End API to create a google account
function callApiRegister(jwt: string) {
    const requestOptions = {
        method: 'POST',
        headers: { 'Authorization': `Bearer ${jwt}` },
    };

    fetch('https://localhost:8001/api/Account/RegisterGoogle', requestOptions)
        .then(response => response.json())
        .then(json => {
            console.log('Login response: ', json)
        });
}

// Calls Back-End API to create a get account JWT
function callApiLogin(jwt: string) {
    const requestOptions = {
        method: 'GET',
        headers: { 
            'Authorization': `Bearer ${jwt}`,
            'Content-Type': 'application/json'
         },
    };

    fetch('https://localhost:8001/api/Account/LoginGoogle', requestOptions)
        .then(response => response.json())
        .then(json => {
            console.log('Login response: ', json)
        });
}

function parseJwt(token: string) {
    var base64Url = token.split('.')[1];
    var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    var jsonPayload = decodeURIComponent(atob(base64).split('').map(function (c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));

    return JSON.parse(jsonPayload);
};