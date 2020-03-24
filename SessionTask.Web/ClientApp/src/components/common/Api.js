import axios from 'axios'
import { authenticationService } from '../services/authentication.service'; 



const Api = axios.create({
    baseURL: process.env.REACT_APP_API_URL
});
Api.defaults.headers.post['Content-Type'] = 'application/json';

Api.interceptors.request.use(function (config) {
    const token = JSON.parse(localStorage.getItem('currentUser') || '{}')['token']
    Api.defaults.headers.common['Authorization'] = `Bearer ${token}`
    return config;
});

Api.interceptors.response.use(function (response) {
    return response;
}, function (error) {
        if ([401, 403].indexOf(error.response.status) !== -1) {
        // auto logout if 401 Unauthorized or 403 Forbidden response returned from api
        authenticationService.logout();
        window.location.reload(true);
    }
    return Promise.reject(error);
});

export default Api 
