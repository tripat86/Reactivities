import { Resolver, useForm } from "react-hook-form";
import { useAccount } from "../../lib/hooks/useAccount";
import { loginSchema, LoginSchema } from "../../lib/schemas/loginSchema";
import { zodResolver } from "@hookform/resolvers/zod";
import { Box, Button, Paper, Typography } from "@mui/material";
import { LockOpen } from "@mui/icons-material";
import TextInput from "../../app/shared/components/TextInput";
import { Link, useLocation, useNavigate } from "react-router";

export default function LoginForm() {
  const navigate = useNavigate();
  const location = useLocation();
  const { loginUser } = useAccount();
  const { control, handleSubmit, formState: { isValid, isSubmitting } } = useForm<LoginSchema>({
    mode: 'onTouched',
    resolver: zodResolver(loginSchema) as Resolver<LoginSchema>,
    defaultValues: {
      email: '',
      password: ''
    } as LoginSchema
  });

  const onSubmit = async (data: LoginSchema) => {
    await loginUser.mutateAsync(data, {
      onSuccess: () => {
        // redirect to home page or previous page
        navigate(location.state?.from || '/activities');
      }
    });
  }

  return (
    <Paper
      component={"form"}
      onSubmit={handleSubmit(onSubmit)}
      sx={{
        display: 'flex',
        flexDirection: 'column',
        gap: 3,
        p: 3,
        maxWidth: 'md',
        mx: 'auto',
        borderRadius: 3
      }}
    >
      <Box display='flex' alignItems='center' justifyContent='center'
        gap={3} color='secondary.main'>
        <LockOpen fontSize="large" />
        <Typography variant="h4">Sign in</Typography>
      </Box>
      <TextInput label="Email" name="email" control={control} />
      <TextInput label="Password" type='password' name="password" control={control} />
      <Button
        type="submit"
        disabled={!isValid || isSubmitting}
        variant="contained"
        size="large"
      > Login
      </Button>
      <Typography sx={{ textAlign: 'center' }}>
        Don't have an account? 
        <Typography sx={{ml:2}}component={Link} to='/register' color='primary'>
          Sign up
        </Typography>
        
      </Typography>
    </Paper>
  )
}