import React, { useState, useEffect, useRef, Fragment } from 'react';
import { makeStyles } from '@material-ui/core/styles';
import Grid from '@material-ui/core/Grid';
import Paper from '@material-ui/core/Paper';
import { AgGridReact } from 'ag-grid-react';
import EditIcon from '@material-ui/icons/Edit';
import Fab from '@material-ui/core/Fab';
import * as AppConstants from './constants/ApplicationConstants';
import * as Constants from './constants/ApiMethods'
import Api from './common/Api'
import TextField from '@material-ui/core/TextField';
import Button from '@material-ui/core/Button';
import Dialog from '@material-ui/core/Dialog';
import DialogActions from '@material-ui/core/DialogActions';
import DialogContent from '@material-ui/core/DialogContent';
import DialogTitle from '@material-ui/core/DialogTitle';
import SaveIcon from '@material-ui/icons/Save';
import CancelIcon from '@material-ui/icons/Cancel';
import AddIcon from '@material-ui/icons/Add';
import { useSnackbar } from 'notistack';
import Moment from 'moment';
import VisibilityIcon from '@material-ui/icons/Visibility';
import AddToQueueIcon from '@material-ui/icons/AddToQueue';
import DoneIcon from '@material-ui/icons/Done';
import DoneAllIcon from '@material-ui/icons/DoneAll';

const useStyles = makeStyles(theme => ({
    mLAuto: {
        marginLeft: 'auto',
    }
}));

export default function EventSummary() {
    const { enqueueSnackbar } = useSnackbar();
    const classes = useStyles();

    const [events, setEvents] = useState([]);
    const [sessions, setSessions] = useState([]);
    const [sessionAttendees, setSessionAttendees] = useState([]);

    const [isEventModalOpen, setIsEventModalOpen] = useState(false);
    const [isSessionModalOpen, setIsSessionModalOpen] = useState(false);
    const [isAttendeesModalOpen, setIsAttendeesModalOpen] = useState(false);
    const [isEnrollConfirmationModalOpen, SetIsEnrollConfirmationModalOpen] = useState(false);

    const initialEventDetails = {
        eventId: 0,
        eventName: null,
        eventDescription: null,
        eventDate: Moment().format("YYYY-MM-DD"),
        startTime: Moment().format("HH:mm"),
        endTime: Moment().format("HH:mm"),
        maxCount: null
    };

    const [eventDetails, setEventDetails] = useState({ ...initialEventDetails });

    const initialSessionDetails = {
        sessionId: 0,
        eventId: eventDetails.eventId,
        sessionName: null,
        Description: null,
        hostName: null,
        startTime: Moment().format("HH:mm"),
        endTime: Moment().format("HH:mm"),
        maxCount: null
    }
    const [sessionDetails, setSessionDetails] = useState({ ...initialSessionDetails });

    useEffect(() => {
        getEvents();
    }, []);

    const checkAccess = (featureName, permissionName) => {
        const userDetails = JSON.parse(localStorage.getItem('currentUser'));
        const filteredFeatures = userDetails.features.filter(feature => feature.featureName == featureName && feature.permission == permissionName);
        //if (permissionName && filteredFeatures.length > 0) {
        //    const filteredPermissions = filteredFeatures[0].permissions.filter(permission => permission.permissionName == permissionName);
        //    return filteredPermissions.length > 0
        //}
        return filteredFeatures.length > 0;
    }

    const canApproveAttendees = checkAccess('Attendees', 'Update')
    const hasEditSessionAccess = checkAccess('Session', 'Update');
    const hasViewAttendeesAccess = checkAccess('Attendees', 'Read');
    const canEnrollToSession = checkAccess('Enroll', 'Create');
    const canCreateEvent = checkAccess('Event', 'Create')
    const canCreateSession = checkAccess('Session', 'Create')
    const canUpdateEvent = checkAccess('Event', 'Update')

    const eventGridApi = useRef(null);

    const onEventGridReady = params => {
        eventGridApi.current = params;
    }

    //Event grid columns
    const eventGridColumnDefs = [
        {
            headerName: "Id", field: "id", sortable: true, filter: true, hide: true,
            flex: 0
        },
        {
            headerName: "Name", field: "eventName", sortable: true, filter: true,
            flex: 2
        },
        {
            headerName: "Date", field: "eventDate", sortable: true, filter: true,
            flex: 2,
            valueGetter: function (params) {
                return params.data.eventDate ? Moment(params.data.eventDate).format('MM/DD/YYYY') : '';
            }
        },
        {
            headerName: "Start Time", field: "startTime", sortable: true, filter: true,
            flex: 1,
            valueGetter: function (params) {
                return params.data.startTime ? Moment(params.data.startTime).format('HH:mm') : '';
            }
        },
        {
            headerName: "End Time", field: "endTime", sortable: true, filter: true,
            flex: 1,
            valueGetter: function (params) {
                return params.data.endTime ? Moment(params.data.endTime).format('HH:mm') : '';
            }
        },
        {
            headerName: "Max Count", field: "maxCount", sortable: true, filter: true,
            flex: 1
        }
    ];

    if (canUpdateEvent) {
        //If user has access to Edit event then show the edit button
        eventGridColumnDefs.push({
            headerName: "Actions", field: "id",
            cellRenderer: "actionsColumnRenderer",
            flex: 2
        });
    }

    const EventActionsColumnRenderer = (params) => {
        return (
            <Fab color="secondary" size="small" aria-label="edit"
                onClick={() => handleEventEditClick(params.value)}
            >
                <EditIcon />
            </Fab>
        );
    }

    const handleEventEditClick = (value) => {
        const selectedRows = eventGridApi.current.api.getSelectedRows();
        const eventDetailsObj = { ...selectedRows[0] }
        eventDetailsObj.startTime = Moment(eventDetailsObj.startTime).format('HH:mm');
        eventDetailsObj.endTime = Moment(eventDetailsObj.endTime).format('HH:mm');
        eventDetailsObj.eventDate = Moment(eventDetailsObj.eventDate).format('YYYY-MM-DD');
        setEventDetails(eventDetailsObj);
        handleEventModalOpen();
    }

    const onEventGridRowSelectionChanged = () => {
        getSessions();
    }

    const getEvents = async () => {
        if (eventGridApi.current) {
            eventGridApi.current.api.showLoadingOverlay();
        }
        const response = await Api.get(Constants.getEvents);
        setEvents(response.data)

        if (response.data.length === 0) {
            eventGridApi.current.api.showNoRowsOverlay();
        } else {
            eventGridApi.current.api.hideOverlay();
        }
    }

    const handleCreateNewEvent = () => {
        setEventDetails({ ...initialEventDetails });
        handleEventModalOpen();
    }

    const handleEventModalOpen = () => {
        setIsEventModalOpen(true);
    };

    const handleEventModalClose = () => {
        setIsEventModalOpen(false);
    };

    const handleEventDetailsChange = (event, name) => {
        setEventDetails({ ...eventDetails, [name]: event.target.value });
    }

    const handleSaveEventDetails = () => {
        const eventDetailsObj = { ...eventDetails };
        eventDetailsObj.startTime = Moment(eventDetailsObj.eventDate + ' ' + eventDetailsObj.startTime).format('YYYY-MM-DDTHH:mm:ss');
        eventDetailsObj.endTime = Moment(eventDetailsObj.eventDate + ' ' + eventDetailsObj.endTime).format('YYYY-MM-DDTHH:mm:ss');
        eventDetailsObj.eventDate = Moment(eventDetailsObj.eventDate).format('YYYY-MM-DDTHH:mm:ss');
        return Api.post(Constants.saveEvent, eventDetailsObj).then((resp) => {
            enqueueSnackbar('Event saved successfully', { variant: 'success' });
            handleEventModalClose();
            getEvents()
        }).catch(error => {
            enqueueSnackbar('An error occurred while saving event', { variant: 'error' });
            console.log(error.response.data);
        });
    }

    // Sessions

    const sessionGridApi = useRef(null);

    const onSessionGridReady = params => {
        sessionGridApi.current = params;
    }

    const sessionGridColumnDefs = [
        {
            headerName: "Id", field: "id", sortable: true, filter: true, hide: true,
            flex: 0
        },
        {
            headerName: "Session Name", field: "sessionName", sortable: true, filter: true,
            flex: 2
        },
        {
            headerName: "Host Name", field: "hostName", sortable: true, filter: true,
            flex: 2
        },
        {
            headerName: "Start Time", field: "startTime", sortable: true, filter: true,
            flex: 1,
            valueGetter: function (params) {
                return params.data.startTime ? Moment(params.data.startTime).format('HH:mm') : '';
            }
        },
        {
            headerName: "End Time", field: "endTime", sortable: true, filter: true,
            flex: 1,
            valueGetter: function (params) {
                return params.data.endTime ? Moment(params.data.endTime).format('HH:mm') : '';
            }
        },
        {
            headerName: "Max Count", field: "maxCount", sortable: true, filter: true,
            flex: 1
        },
        {
            headerName: "Actions", field: "id",
            cellRenderer: "actionsColumnRenderer",
            flex: 2
        }
    ];

    const SessionActionsColumnRenderer = (params) => {
        
        return (
            <Fragment>
                {
                    hasEditSessionAccess &&
                    <Fab color="secondary" size="small" aria-label="edit" title="Edit Session"
                        onClick={handleSessionEditClick}
                    >
                        <EditIcon />
                    </Fab>
                }
                {
                    hasViewAttendeesAccess &&
                    <Fab className={classes.fab} color="primary" size="small" aria-label="view" title="View Attendees"
                    onClick={handleAttendeesModalOpen}
                    >
                    <VisibilityIcon />
                </Fab>
                }
                {
                    canEnrollToSession && !params.data.isEnrolled &&
                    <Fab className={classes.fab} color="primary" size="small" aria-label="view" title="Enroll"
                        onClick={handleEnrollSessionConfirmation}
                    >
                        <AddToQueueIcon />
                    </Fab>
                }
                {
                    canEnrollToSession && params.data.isEnrolled && !params.data.isApproved &&
                    <Fab className={classes.fab} color="primary" size="small" aria-label="view" title="Enrolled"
                    >
                        <DoneIcon />
                    </Fab>
                    
                }
                {
                    canEnrollToSession && params.data.isApproved &&
                    <Fab className={classes.fab} color="primary" size="small" aria-label="view" title="Approved"
                    >
                        <DoneAllIcon />
                    </Fab>
                   
                }
            </Fragment>
        );
    }

    const getSessions = async () => {
        const selectedRows = eventGridApi.current.api.getSelectedRows();
        const { eventId } = selectedRows[0];
        if (sessionGridApi.current) {
            sessionGridApi.current.api.showLoadingOverlay();
        }
        const userDetails = JSON.parse(localStorage.getItem('currentUser'));
        const response = await Api.get(Constants.getSessions, {
            params: {
                eventId: eventId,
                userId: userDetails.userId
            }
        });
        setSessions(response.data)

        if (response.data.length === 0) {
            sessionGridApi.current.api.showNoRowsOverlay();
        } else {
            sessionGridApi.current.api.hideOverlay();
        }
    }

    const handleSessionModalOpen = () => {
        setIsSessionModalOpen(true);
    };

    const handleSessionModalClose = () => {
        setIsSessionModalOpen(false);
    };

    const handleSaveSessionDetails = () => {
        const sessionDetailsObj = { ...sessionDetails };
        const selectedEventRows = eventGridApi.current.api.getSelectedRows();
        sessionDetailsObj.eventId = selectedEventRows[0].eventId;
        sessionDetailsObj.startTime = Moment(eventDetails.eventDate + ' ' + sessionDetailsObj.startTime).format('YYYY-MM-DDTHH:mm:ss');
        sessionDetailsObj.endTime = Moment(eventDetails.eventDate + ' ' + sessionDetailsObj.endTime).format('YYYY-MM-DDTHH:mm:ss');
        Api.post(Constants.saveSession, sessionDetailsObj).then((resp) => {
            enqueueSnackbar('Session saved successfully', { variant: 'success' });
            handleSessionModalClose();
            getSessions()
        }).catch(error => {
            enqueueSnackbar('An error occurred while saving session', { variant: 'error' });
            console.log(error.response.data);
        });
    }

    const handleSessionDetailsChange = (event, name) => {
        setSessionDetails({ ...sessionDetails, [name]: event.target.value });
    }

    const handleSessionEditClick = () => {
        const selectedRows = sessionGridApi.current.api.getSelectedRows();
        const sessionDetailsObj = { ...selectedRows[0] }
        sessionDetailsObj.startTime = Moment(sessionDetailsObj.startTime).format('HH:mm');
        sessionDetailsObj.endTime = Moment(sessionDetailsObj.endTime).format('HH:mm');
        setSessionDetails(sessionDetailsObj);
        handleSessionModalOpen();
    }

    const handleAddSession = () => {
        setSessionDetails({ ...initialSessionDetails });
        handleSessionModalOpen();
    }

    const CreateEvent = () => {
       
        return (canCreateEvent && <Fab color="primary" size="small" title="Create Event" onClick={handleCreateNewEvent}>
            <AddIcon />
        </Fab>)
    }

    const AddSession = () => {
        
        return (canCreateSession && <Fab color="primary" size="small" title="Add Session" onClick={handleAddSession}>
            <AddIcon />
        </Fab>)
    }

    //Attendees

    const attendeesGridApi = useRef(null);

    const onAttendeesGridReady = params => {
        attendeesGridApi.current = params;
    }

    const handleAttendeesModalOpen = async () => {
        setIsAttendeesModalOpen(true);
        const selectedRows = sessionGridApi.current.api.getSelectedRows();
        const { sessionId } = selectedRows[0]
        const userDetails = JSON.parse(localStorage.getItem('currentUser'));
        if (attendeesGridApi.current) {
            attendeesGridApi.current.api.showLoadingOverlay();
        }
        const response = await Api.get(Constants.getSessionAttendees, {
            params: {
                sessionId: sessionId,
                userId: userDetails.userId
            }
        });
        setSessionAttendees(response.data)

        if (response.data.length === 0) {
            attendeesGridApi.current.api.showNoRowsOverlay();
        } else {
            attendeesGridApi.current.api.hideOverlay();
        }
    };

    const handleAttendeesModalClose = () => {
        setIsAttendeesModalOpen(false);
    };

    const sessionAttendeesGridColumnDefs = [
        {
            headerName: "Id", field: "userId", sortable: true, filter: true, hide: true
        },
        {
            headerName: "Attendee", field: "attendeeName", sortable: true, filter: true,
            headerCheckboxSelection: true,
            headerCheckboxSelectionFilteredOnly: true,
            checkboxSelection: function (params) {
                return canApproveAttendees && !params.data.isApproved;
            },
            flex: 2
        },
        {
            headerName: "Session Name", field: "sessionName", sortable: true, filter: true,
            flex: 3
        },
        {
            headerName: "Event Name", field: "eventName", sortable: true, filter: true,
            flex: 3
        },
        {
            headerName: "Host Name", field: "hostName", sortable: true, filter: true,
            flex: 3
        },
        {
            headerName: "Status", field: "isApproved", sortable: true, filter: true,
            flex: 3,
            valueGetter: function (params) {
                return params.data.isApproved ? 'Approved' : 'Pending Approval';
            }
        }
    ];

    const handleApprove = () => {
        const selectedAttendees = attendeesGridApi.current.api.getSelectedNodes();
        const selectedRows = sessionGridApi.current.api.getSelectedRows();
        const { sessionId } = selectedRows[0]
        if (selectedAttendees.length > 0) {
            Api.post(Constants.approveAttendees, {
                UserIds: selectedAttendees.map(x => x.data.userId),
                SessionId: sessionId
            })
                .then(() => {
                    handleAttendeesModalClose();
                    enqueueSnackbar('Approved successfully', { variant: 'success' });
                })
                .catch(error => {
                    enqueueSnackbar('An error occurred while approving attendees', { variant: 'error' });
                    console.log(error);
                });
        } else {
            enqueueSnackbar('Please select atleast one attendee', { variant: 'error' });
        }
    }

    const handleEnrollSessionConfirmation = () => {
        handleEnrollConfirmationModalOpen();
    }

    const handleEnrollConfirmationModalOpen = () => {
        SetIsEnrollConfirmationModalOpen(true);
    };

    const handleEnrollConfirmationModalClose = () => {
        SetIsEnrollConfirmationModalOpen(false);
    };

    const handleEnrollToSession = () => {
        const selectedRows = sessionGridApi.current.api.getSelectedRows();
        const { sessionId } = selectedRows[0];
        const userDetails = JSON.parse(localStorage.getItem('currentUser'));
        Api.post(Constants.enrollToSession, {
            UserId: userDetails.userId,
            SessionId: sessionId
        }).then((resp) => {
            enqueueSnackbar('Enrolled successfully', { variant: 'success' });
            handleEnrollConfirmationModalClose();
            getSessions();
        }).catch(error => {
            enqueueSnackbar('An error occurred while enrolling to session', { variant: 'error' });
            console.log(error.response.data);
        });
    }

    const ApproveSessionAttendees = () => {
        return (
            canApproveAttendees &&
            <Button onClick={handleApprove}
            variant="contained"
            color="primary"
            size="small"
            className={classes.button}
            startIcon={<SaveIcon />}>
            Approve
           </Button>)
    }

    return (
        <Grid container spacing={3}>
            <Grid item xs={12} md={6} lg={6}>
                <Grid container spacing={3} direction="column">
                    <Grid item xs={12} md={10} lg={10}>
                    </Grid>
                </Grid>
            </Grid>
            <Grid item xs={12} md={12} lg={12} spacing={3}>
                <div>
                    <CreateEvent />
                    <Dialog open={isEventModalOpen} fullWidth disableBackdropClick onClose={handleEventModalClose}
                        maxWidth='md' >
                        <DialogTitle >{eventDetails.eventId ? 'Edit' : 'Create New'} Event</DialogTitle>
                        <DialogContent>
                            <TextField
                                margin="normal"
                                variant="outlined"
                                label="Event Name"
                                fullWidth
                                inputProps={{
                                    spellCheck: true
                                }}
                                value={eventDetails.eventName || ''}
                                onChange={(e) => handleEventDetailsChange(e, 'eventName')}
                            />
                            <TextField
                                margin="normal"
                                variant="outlined"
                                label="Description"
                                rows={3}
                                rowsMax={4}
                                fullWidth
                                multiline
                                inputProps={{
                                    spellCheck: true
                                }}
                                value={eventDetails.eventDescription || ''}
                                onChange={(e) => handleEventDetailsChange(e, 'eventDescription')}
                            />
                            <TextField
                                margin="normal"
                                variant="outlined"
                                label="Event Date"
                                type="date"
                                InputLabelProps={{
                                    shrink: true,
                                }}
                                value={eventDetails.eventDate || ''}
                                onChange={(e) => handleEventDetailsChange(e, 'eventDate')}
                            />
                            <TextField
                                margin="normal"
                                variant="outlined"
                                label="Start Time"
                                type="time"
                                InputLabelProps={{
                                    shrink: true,
                                }}
                                inputProps={{
                                    step: 300, // 5 min
                                }}
                                value={eventDetails.startTime || ''}
                                onChange={(e) => handleEventDetailsChange(e, 'startTime')}
                            />
                            <TextField
                                margin="normal"
                                variant="outlined"
                                label="End Time"
                                type="time"
                                InputLabelProps={{
                                    shrink: true,
                                }}
                                inputProps={{
                                    step: 300, // 5 min
                                }}
                                value={eventDetails.endTime || ''}
                                onChange={(e) => handleEventDetailsChange(e, 'endTime')}
                            />
                            <TextField
                                margin="normal"
                                variant="outlined"
                                label="Max Count"
                                type="number"
                                fullWidth
                                InputProps={{ inputProps: { min: 0, max: 500 } }}
                                value={eventDetails.maxCount || ''}
                                onChange={(e) => handleEventDetailsChange(e, 'maxCount')}
                            />
                        </DialogContent>
                        <DialogActions>
                            <Button
                                onClick={handleSaveEventDetails}
                                variant="contained"
                                color="primary"
                                size="small"
                                className={classes.button}
                                startIcon={<SaveIcon />}>
                                Save
                        </Button>
                            <Button
                                onClick={handleEventModalClose}
                                variant="contained"
                                size="small"
                                startIcon={<CancelIcon />}>
                                Cancel
                    </Button>
                        </DialogActions>
                    </Dialog>
                </div>
                <Grid item xs={12} md={12} lg={12}>
                    <Paper style={{ height: '50vh', width: '100%' }} >
                        <Grid container spacing={3} justify="flex-start" alignItems="center">
                            <Grid item xs={4} md={4} lg={4}>
                            </Grid>
                            <Grid item className={classes.mLAuto}>

                            </Grid>
                        </Grid>

                        <div style={{ height: '80%', width: '100%' }} className="ag-theme-balham">
                            <AgGridReact
                                columnDefs={eventGridColumnDefs}
                                rowData={events}
                                rowHeight={50}
                                onGridReady={onEventGridReady}
                                rowSelection="single"
                                onSelectionChanged={onEventGridRowSelectionChanged}
                                frameworkComponents={{
                                    actionsColumnRenderer: EventActionsColumnRenderer,
                                }}
                                defaultColDef={{
                                    resizable: true
                                }}
                                overlayLoadingTemplate={AppConstants.overlayLoadingTemplate}
                                overlayNoRowsTemplate={AppConstants.overlayNoRowsTemplate}
                            />
                        </div>
                    </Paper>
                </Grid >
                <div>
                    <AddSession />
                    <Dialog open={isSessionModalOpen} fullWidth disableBackdropClick onClose={handleSessionModalClose}
                        maxWidth='md' >
                        <DialogTitle >{sessionDetails.sessionId ? 'Edit' : 'Add New'} Session</DialogTitle>
                        <DialogContent>
                            <TextField
                                margin="normal"
                                variant="outlined"
                                label="Session Name"
                                fullWidth
                                value={sessionDetails.sessionName || ''}
                                onChange={(e) => handleSessionDetailsChange(e, 'sessionName')}
                            />
                            <TextField
                                margin="normal"
                                variant="outlined"
                                label="Description"
                                rows={3}
                                rowsMax={4}
                                fullWidth
                                multiline
                                inputProps={{
                                    spellCheck: true
                                }}
                                value={sessionDetails.description || ''}
                                onChange={(e) => handleSessionDetailsChange(e, 'description')}
                            />
                            <TextField
                                margin="normal"
                                variant="outlined"
                                label="Host Name"
                                fullWidth
                                value={sessionDetails.hostName || ''}
                                onChange={(e) => handleSessionDetailsChange(e, 'hostName')}
                            />
                            <TextField
                                margin="normal"
                                variant="outlined"
                                label="Start Time"
                                type="time"
                                InputLabelProps={{
                                    shrink: true,
                                }}
                                inputProps={{
                                    step: 300, // 5 min
                                }}
                                value={sessionDetails.startTime || ''}
                                onChange={(e) => handleSessionDetailsChange(e, 'startTime')}
                            />
                            <TextField
                                margin="normal"
                                variant="outlined"
                                label="End Time"
                                type="time"
                                InputLabelProps={{
                                    shrink: true,
                                }}
                                inputProps={{
                                    step: 300, // 5 min
                                }}
                                value={sessionDetails.endTime || ''}
                                onChange={(e) => handleSessionDetailsChange(e, 'endTime')}
                            />
                            <TextField
                                margin="normal"
                                variant="outlined"
                                label="Max Count"
                                type="number"
                                fullWidth
                                InputProps={{ inputProps: { min: 0, max: 500 } }}
                                value={sessionDetails.maxCount || ''}
                                onChange={(e) => handleSessionDetailsChange(e, 'maxCount')}
                            />
                        </DialogContent>
                        <DialogActions>
                            <Button
                                onClick={handleSaveSessionDetails}
                                variant="contained"
                                color="primary"
                                size="small"
                                className={classes.button}
                                startIcon={<SaveIcon />}>
                                Save
                            </Button>
                            <Button onClick={handleSessionModalClose}
                                variant="contained"
                                size="small"
                                startIcon={<CancelIcon />}>
                                Cancel
                    </Button>
                        </DialogActions>
                    </Dialog>
                </div>
                <Grid item xs={12} md={12} lg={12}>
                    <Grid item xs={12} md={12} lg={12}>
                        <Paper style={{ height: '50vh' }}>
                            <Grid container spacing={3} justify="flex-start" alignItems="center">
                                <Grid item xs={4} md={4} lg={4}>

                                </Grid>
                                <Grid item className={classes.mLAuto}>

                                </Grid>
                            </Grid>
                            <div style={{ height: '80%', width: '100%' }} className="ag-theme-balham">
                                <AgGridReact
                                    columnDefs={sessionGridColumnDefs}
                                    rowData={sessions}
                                    rowHeight={50}
                                    onGridReady={onSessionGridReady}
                                    rowSelection="single"
                                    frameworkComponents={{
                                        actionsColumnRenderer: SessionActionsColumnRenderer,
                                    }}
                                    defaultColDef={{
                                        resizable: true
                                    }}
                                    overlayLoadingTemplate={AppConstants.overlayLoadingTemplate}
                                    overlayNoRowsTemplate={AppConstants.overlayNoRowsTemplate}
                                />
                            </div>
                        </Paper>
                    </Grid>
                </Grid>
                <div>
                    <Dialog open={isAttendeesModalOpen} fullWidth disableBackdropClick onClose={handleAttendeesModalClose}
                        maxWidth='md' >
                        <DialogTitle >Session Attendees</DialogTitle>
                        <DialogContent>
                            <div style={{ height: '50vh' }}>
                                <div style={{ height: '100%', width: '100%' }} className="ag-theme-balham">
                                    <AgGridReact
                                        columnDefs={sessionAttendeesGridColumnDefs}
                                        rowData={sessionAttendees}
                                        rowHeight={50}
                                        onGridReady={onAttendeesGridReady}
                                        defaultColDef={{
                                            resizable: true
                                        }}
                                        overlayLoadingTemplate={AppConstants.overlayLoadingTemplate}
                                        overlayNoRowsTemplate={AppConstants.overlayNoRowsTemplate}
                                    />
                                </div>
                            </div>
                        </DialogContent>
                        <DialogActions>
                            <ApproveSessionAttendees/>
                            <Button onClick={handleAttendeesModalClose}
                                variant="contained"
                                size="small"
                                startIcon={<CancelIcon />}>
                                Cancel
                    </Button>
                        </DialogActions>
                    </Dialog>
                </div>
                <div>
                    <Dialog open={isEnrollConfirmationModalOpen} fullWidth disableBackdropClick
                        onClose={handleEnrollConfirmationModalClose}
                        maxWidth='md' >
                        <DialogTitle >Enroll Confirmation</DialogTitle>
                        <DialogContent>
                            Are you sure you want to enroll to this session ?
                        </DialogContent>
                        <DialogActions>
                            <Button
                                onClick={handleEnrollToSession}
                                variant="contained"
                                color="primary"
                                size="small"
                                className={classes.button}
                                startIcon={<SaveIcon />}>
                                Enroll
                            </Button>
                            <Button onClick={handleEnrollConfirmationModalClose}
                                variant="contained"
                                size="small"
                                startIcon={<CancelIcon />}>
                                Cancel
                    </Button>
                        </DialogActions>
                    </Dialog>
                </div>
            </Grid>
        </Grid>
    );
}