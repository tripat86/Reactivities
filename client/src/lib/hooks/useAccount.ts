import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"
import { LoginSchema } from "../schemas/loginSchema";
import agent from "../api/agent";
import { useLocation, useNavigate } from "react-router";
import { RegisterSchema } from "../schemas/registerSchema";
import { toast } from "react-toastify";

export const useAccount = () => {
    const queryClient = useQueryClient();
    const navigate = useNavigate();
    const location = useLocation();

    const loginUser = useMutation({
        mutationFn: async (creds: LoginSchema) => {
            // call the login API endpoint  
            await agent.post('/login?useCookies=true', creds);
        },
        onSuccess: async () => {
            await queryClient.invalidateQueries({ queryKey: ['user'] });
        }
    });

    const registerUser = useMutation({
        mutationFn: async (creds: RegisterSchema) => {
            // call the login API endpoint
            await agent.post('/account/register', creds)
        },
        onSuccess: () => {
            toast.success('Registration successful - you can now login');
            navigate('/login');
        }
    });

    const logoutUser = useMutation({
        mutationFn: async () => {
            await agent.post('/account/logout');
        },
        onSuccess: async () => {
            await queryClient.removeQueries({ queryKey: ['user'] });
            await queryClient.removeQueries({ queryKey: ['activities'] });
            await navigate('/');
        }
    });

    // It means data is aliased to currentUser    
    const { data: currentUser, isLoading: loadingUserInfo } = useQuery({
        queryKey: ['user'],
        queryFn: async () => {
            const response = await agent.get<User>('/account/user-info');
            return response.data;
        },
        enabled: !queryClient.getQueryData(['user']) && location.pathname !== '/login' && location.pathname !== '/register' // only fetch if we don't have user data already in react-query cache data
    })

    return {
        loginUser,
        currentUser,
        logoutUser,
        loadingUserInfo,
        registerUser
    }
}