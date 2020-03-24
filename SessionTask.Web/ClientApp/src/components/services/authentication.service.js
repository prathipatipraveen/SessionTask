import { BehaviorSubject } from 'rxjs';
import Api from '../common/Api'
import * as Constants from '../constants/ApiMethods'

const currentUserSubject = new BehaviorSubject(JSON.parse(localStorage.getItem('currentUser')));

export const authenticationService = {
    login,
    logout,
    currentUser: currentUserSubject.asObservable(),
    get currentUserValue() { return currentUserSubject.value }
};

function login(username, password) {
    return Api.post(Constants.signIn, JSON.stringify({ username, password })).then((resp) => {
        localStorage.setItem('currentUser', JSON.stringify(resp.data));
        currentUserSubject.next(resp);
        return resp;
    }).catch(error => {
    });
}

function logout() {
    // remove user from local storage to log user out
    localStorage.removeItem('currentUser');
    currentUserSubject.next(null);
}
