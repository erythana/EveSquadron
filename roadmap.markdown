# Roadmap
Some things to come in the future.

### More detailed Information

With the new backend, it will be possible to display more detailed information, such as
* Average size of the attackers group
* Kills/Losses over the last week
* ... and probably more

### Caching and Cache Invalidation

At the moment, every clipboard copy query results in a query to the API.<br />
This can cause a slight delay, especially when there are a lot of people in local.<br />
To speed things up, caching and cache invalidation will be implemented.

### UI Improvements

There are several UI improvements planned for the user interface of this tool.<br />
In particular, the character tooltip with the details needs some work.<br />
Additionally, window sizes will be persisted after the app is closed.