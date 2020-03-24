import React from 'react';
import CssBaseline from '@material-ui/core/CssBaseline';
import 'ag-grid-community/dist/styles/ag-grid.css';
import 'ag-grid-community/dist/styles/ag-theme-balham.css';
import { LoginPage } from './components/LoginPage'
import { makeStyles } from '@material-ui/core/styles';
import { Router, Route, Link } from 'react-router-dom';
import { AdminPage } from './components/Admin';
import { authenticationService } from './components/services/authentication.service';
import { createBrowserHistory } from 'history';
import { PrivateRoute } from './components/PrivateRoute';
import EventSummary from './components/EventSummary';

const useStyles = makeStyles(() => ({
    root: {
        display: 'flex',
    }
}));

const history = createBrowserHistory();
//const classes = useStyles();

class App extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            currentUser: null,
            isAdmin: false
        };
    }

    componentDidMount() {
        authenticationService.currentUser.subscribe(x => this.setState({
            currentUser: x,
            isAdmin: x && x.role === "Admin"
        }));
    }

    logout() {
        authenticationService.logout();
        history.push('/login');
    }

    render() {
        const { currentUser, isAdmin } = this.state;
        return (
            <Router history={history}>
                <div >
                    <CssBaseline />
                    {currentUser &&
                        <nav className="navbar navbar-expand navbar-dark bg-dark">
                            <div className="navbar-nav">
                                <Link to="/" className="nav-item nav-link">Home</Link>
                                {isAdmin && <Link to="/admin" className="nav-item nav-link">Admin</Link>}
                                <a onClick={this.logout} className="nav-item nav-link">Logout</a>
                            </div>
                        </nav>
                    }
                    <PrivateRoute exact path="/" component={EventSummary} />
                    <PrivateRoute path="/admin" roles={['Admin']} component={AdminPage} />
                    <Route path="/login" component={LoginPage} />
                </div>
            </Router>
        );
    }
}

export { App }; 
