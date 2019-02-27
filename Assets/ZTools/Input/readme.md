
inputProvider1 ----                               ---- inputHandler1
                    \.____ input manager ______.
inputProvider2 ----                             \ ---- inputHandler2


- inputProvider is wrapper layer, which convert different types of input data into one.
- inputHandler is customized layer, which convert raw input data into its special input msg or function call.
- inputManager can switch between those inputProviders, the same for inputHandlers.

- inputProvider can be unity input msg, easy-touch plugin msg, AI msg, etc.
- one uniform data should contain enough information to represent all kinds of operation.
