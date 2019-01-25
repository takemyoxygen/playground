import {parse, stringify} from 'query-string';

function removeLeadingSlashFrom(input) {
    return input[0] === '/' ? input.substring(1) : input;
}

function appendLeadingSlashTo(input) {
    return input
        ? input[0] === '/'
            ? input
            : '/' + input
        : '/';
}

export default function(baseHistory, param) {
    const initial = parse(removeLeadingSlashFrom(baseHistory.location.pathname));

    let listeners = [];


    const history = {
        length: baseHistory.length,
        location: {
            ...baseHistory.location,
            pathname: appendLeadingSlashTo(initial[param])
        },
        action: baseHistory.action,
        listen(listener) {
            listeners.push(listener);
            return () => {
                listeners = listeners.filter(l => l !== listener);
            }
        },
        push(path, state) {
            const newPath = stringify({
                ...initial,
                [param]: appendLeadingSlashTo(path)
            }, {encode: false});

            return baseHistory.push(newPath, state)
        },
        replace(path, state) {
            return baseHistory.replace(path, state);
        },
        go(n) {
            return baseHistory.go(n);
        },
        goBack() {
            return baseHistory.goBack();
        },
        goForward() {
            return baseHistory.goForward()
        },
        block(blocker) {
            return baseHistory.block(blocker);
        },
        createHref(location) {
            const pathname = stringify({
                ...baseHistory.location,
                [param]: appendLeadingSlashTo(location.pathname)
            }, {encode: false});

            return baseHistory.createHref({
                ...location,
                pathname
            });
        }
    };


    function notify(location, action) {
        console.log(`Notifying ${listeners.length} listeners`);
        listeners.forEach(listener => {
            listener(location, action);
        })
    }

    baseHistory.listen((location, action) => {
        const pathname = appendLeadingSlashTo(parse(removeLeadingSlashFrom(location.pathname))[param]);
        const newLocation = {
            ...location,
            pathname
        };

        console.log('Updating own state', location, action);

        history.location = newLocation;
        history.action = action;
        history.length = baseHistory.length;

        notify(location, action);
    });

    return history;
}