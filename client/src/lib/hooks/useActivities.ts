import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import agent from "../api/agent";
import { useLocation } from "react-router";
import { Activity } from "../types";

export const useActivities = (id?: string) => {
    const queryClient = useQueryClient();
    const location = useLocation();
    // The location object contains the current URL from react router, which can be used to determine if we are

    const { data: activities, isPending } = useQuery({
        queryKey: ['activities'],
        queryFn: async () => {
            const response = await agent.get<Activity[]>('/activities');
            return response.data;
        },
        enabled: !id && location.pathname === '/activities'
        // This means that if the id is provided, we don't fetch all activities
        // and if the current path is not '/activities', we don't fetch all activities
        // This is useful when we are on the activity details page and we don't want to fetch all activities again
    });


    const { data: activity, isLoading: isLoadingActivity } = useQuery({
        queryKey: ['activities', id],
        queryFn: async () => {
            const response = await agent.get<Activity>(`/activities/${id}`);
            return response.data;
        },
        enabled: !!id
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
        isPending,
        updateActivity,
        createActivity,
        deleteActivity,
        activity,
        isLoadingActivity
    }
}