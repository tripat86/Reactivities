import { List, ListItem, ListItemText } from '@mui/material';


type Props= {
  activities:Activity[]
}

export default function ActivityDashboard({activities}:Props) {
  return (
    <List>
    {activities.map((activity) => (
      <ListItem key={activity.id}>
        <ListItemText>{activity.title}</ListItemText>
      </ListItem>
    ))}
    
  </List>
  )
}