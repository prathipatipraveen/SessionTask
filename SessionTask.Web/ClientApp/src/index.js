//import 'bootstrap/dist/css/bootstrap.min.css'
import React from 'react'
import ReactDOM from 'react-dom'
import { BrowserRouter as Router } from 'react-router-dom'
import { App } from './App'
import registerServiceWorker from './registerServiceWorker'
import { SnackbarProvider } from 'notistack';

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
const rootElement = document.getElementById('root');

ReactDOM.render(
    <Router basename={baseUrl}>
        <SnackbarProvider
            autoHideDuration={3000}
            maxSnack={3}
            anchorOrigin={{
                vertical: 'top',
                horizontal: 'right',
            }}
        >
            <App />
        </SnackbarProvider>
    </Router>
    ,
    rootElement);

registerServiceWorker();
