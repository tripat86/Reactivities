import { Box, Button, Paper, Typography } from "@mui/material";
import { useActivities } from "../../../lib/hooks/useActivities";
import { useNavigate, useParams } from "react-router";
import { Resolver, useForm } from 'react-hook-form';
import { useEffect } from "react";
import { zodResolver } from '@hookform/resolvers/zod';
import { activitySchema, ActivitySchema } from "../../../lib/schemas/activitySchema";
import TextInput from "../../../app/shared/components/TextInput";
import SelectInput from "../../../app/shared/components/SelectInput";
import { categoryOptions } from "./CategoryOptions";
import DateTimeInput from "../../../app/shared/components/DateTimeInput";
import LocationInput from "../../../app/shared/components/LocationInput";
import { Activity } from "../../../lib/types";

export default function ActivityForm() {
  const { reset, control, handleSubmit } = useForm<ActivitySchema>({
    mode: 'onTouched',
    resolver: zodResolver(activitySchema) as Resolver<ActivitySchema>,
    defaultValues: {
      title: '',
      description: '',
      category: '',
      date: new Date(),
    } as ActivitySchema
  });
  const navigate = useNavigate();
  const { id } = useParams();
  const { updateActivity, createActivity, activity, isLoadingActivity } = useActivities(id);

  useEffect(() => {
    if (activity) reset({
      ...activity,
      location: {
        city: activity.city,
        venue: activity.venue,
        latitude: activity.latitude,
        longitude: activity.longitude
      }
    });
  }, [activity, reset]);

  const onSubmit = (data: ActivitySchema) => {
    const { location, ...rest } = data;// it will fetch the location object properties and rest will contain the other fields
    const flattenData = { ...rest, ...location } as Activity;// this will combine the location object properties with the rest of the fields

    try {
      if(activity) {
        updateActivity.mutate({...activity, ...flattenData}, {// This will update the existing activity with the updated form data
           onSuccess: () => navigate (`/activities/${activity.id}`) // Navigate to the activity details page after updating
          });// This will update the existing activity with the updated form data
      } else {
        createActivity.mutate(flattenData, {
          onSuccess: (id) => navigate(`/activities/${id}`) // Navigate to the new activity details page after creating
        });// This will create a new activity with the form data
      }
    } catch (error) {
      console.log(error);
    }
  }

  if (isLoadingActivity) return <Typography> Loading Activity ...</Typography>
  return (
    <Paper sx={{ borderRadius: 3, padding: 3 }}>
      <Typography variant="h5" gutterBottom color="primary">
        {activity ? 'Edit Activity' : 'Create Activity'}
      </Typography>
      <Box component='form' onSubmit={handleSubmit(onSubmit)} display='flex' flexDirection='column' gap={3}>
        <TextInput label='Title' control={control} name='title' />
        <TextInput label='Description' control={control} name="description" multiline rows={3} />
        <Box display='flex' justifyContent='space-between' gap={3}>
          <Box flex={1}>
            <SelectInput
              items={categoryOptions}
              label='Category'
              control={control}
              name="category"
            />
          </Box>
          <Box flex={1}>
            <DateTimeInput label='Date' control={control} name="date" />
          </Box>

        </Box>

        <LocationInput control={control} name="location" label="Enter the location" />
        <Box display='flex' justifyContent='end' gap={3}>
          <Button color='inherit'>Cancel</Button>
          <Button
            type="submit"
            color='success'
            variant="contained"
            disabled={updateActivity.isPending || createActivity.isPending}
          >Submit</Button>
        </Box>
      </Box>
    </Paper>
  )
}