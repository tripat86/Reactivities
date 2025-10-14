import axios from "axios";
import { store } from "../stores/store";
import { toast } from "react-toastify";
import { router } from "../../app/router/Routes";

const sleep = (delay: number) => {
    return new Promise(resolve => {
        setTimeout(resolve, delay);
    });
}

const agent = axios.create({
    baseURL: import.meta.env.VITE_API_URL,
    withCredentials: true // this will send the cookies with every request
});

agent.interceptors.request.use(config => {
    // This is where you can set headers, for example, to include a token
    store.uiStore.isBusy();
    return config;
});

agent.interceptors.response.use(
    async response => {
        await sleep(1000);
        store.uiStore.isIdle();
        return response;

    },
    async error => {
        await sleep(1000);
        store.uiStore.isIdle();

        /*error.response is the error object returned by Axios
        You can access the status code, headers, data, etc. from the error object
        For example, to get the status code:*/
        const { status, data} = error.response || {};

        switch (status) {
            case 400:
                if (data.errors) {
                    const modelStateErrors = [];
                    for (const key in data.errors) {
                        if (data.errors[key]) {
                            modelStateErrors.push(data.errors[key]);
                        }
                    }
                    /* If there are model state errors, throw them as a single error
                    flat is used to flatten the array of arrays into a single array*/
                    throw modelStateErrors.flat();
                }
                else {
                    toast.error(data);
                }
                break;
            case 401:
                toast.error('Unauthorized:');
                break;
            case 404:
                router.navigate('/not-found');
                break;
            case 500:
                router.navigate('/server-error',{state: {error: data}});
                break;
            default:
                break;
        }
        // rethrow the error for react query to handle it
        // You can do custom logic here like redirect to /not-found route
        return Promise.reject(error); // ⚠️ must rethrow to propagate
    });

export default agent;

/*
why we use return Promise.reject(error) in Axios interceptors:
When you use an Axios response interceptor:

axios.interceptors.response.use(
  response => { // on success  },
  error => {
    // error handler
    return Promise.reject(error); // ⬅️ this is critical
  }
);
The error handler runs only on error responses (404, 500, etc.)

By doing return Promise.reject(error), you're passing the error down the chain so that:

Your component’s .catch
React Query’s onError
or other interceptors
*/
