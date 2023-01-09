# IPHandlerWebAPI

This API includes a library that uses an external API to retrieve information about an IP that was given by the user.
This information includes IP's continent, country, city, longitude and latitude.
These details are then stored to a database and cache which are used if the user wants to get information about the same IP again.

The user can also update an IP's details by creating a "job" or task.
These tasks are handled by a service that runs in parallel with the API and then provided by a buffer to the main service which updates
the IPs in the database.
Finally a user can check a job's progress at any time by providing its unique key to the API.
