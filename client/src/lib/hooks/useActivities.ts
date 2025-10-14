import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import agent from "../api/agent";
import { useLocation } from "react-router";

import { useAccount } from "./useAccount";

export const useActivities = (id?: string) => {
    const { currentUser } = useAccount();
    const queryClient = useQueryClient();
    const location = useLocation();
    // The location object contains the current URL from react router, which can be used to determine if we are

    const { data: activities, isLoading } = useQuery({
        queryKey: ['activities'],
        queryFn: async () => {
            const response = await agent.get<Activity[]>('/activities');
            return response.data;
        },
        enabled: !id && location.pathname === '/activities' && !!currentUser
        // This means that if the id is provided, we don't fetch all activities
        // and if the current path is not '/activities', we don't fetch all activities
        // This is useful when we are on the activity details page and we don't want to fetch all activities again
        // !!currentUser ensures that we only fetch activities if the user is logged in
        
        // Double !!
        // Using !! is a common pattern to convert any value to a strict boolean without changing its truthiness.

        // -First ! negates it → boolean.

        // -Second ! negates it back → boolean representing the original truthiness.

        // Examples:

        // const currentUser = null;
        // !!currentUser   // false

        // const currentUser = { name: "Tripat" };
        // !!currentUser   // true

        // const currentUser = undefined;
        // !!currentUser   // false
    });


    const { data: activity, isLoading: isLoadingActivity } = useQuery({
        queryKey: ['activities', id],
        queryFn: async () => {
            const response = await agent.get<Activity>(`/activities/${id}`);
            return response.data;
        },
        enabled: !!id && !!currentUser // only fetch if id is provided and user is logged in
    });

    // In React Query, when we need to update/create the record, we use
    // useMutation function
    const updateActivity = useMutation({
        mutationFn: async (activity: Activity) => {
            await agent.put('/activities', activity)
        },

        onSuccess: async () => {
            await queryClient.invalidateQueries({
                queryKey: ['activities']
            })


        }
    })

    const createActivity = useMutation({
        mutationFn: async (activity: Activity) => {
            const response = await agent.post('/activities', activity)
            return response.data;
        },

        onSuccess: async () => {
            await queryClient.invalidateQueries({
                queryKey: ['activities']
            })
        }
    })

    //This id is route parameter
    const deleteActivity = useMutation({
        mutationFn: async (id: string) => {
            await agent.delete(`/activities/${id}`)
        },

        onSuccess: async () => {
            await queryClient.invalidateQueries({
                queryKey: ['activities']
            })
        }
    })


    return {
        activities,
        isLoading,
        updateActivity,
        createActivity,
        deleteActivity,
        activity,
        isLoadingActivity
    }
}