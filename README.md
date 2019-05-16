# Exploring the Paper-based proxy in Augmented Reality Environments

## Install

### Data Visualization - Sankey Graph

```bash
npm install -g http-server  # for loading json file in html 
pip install pandas  # for data processing
```

## How to use?

- Open terminal at the project root path and run command

  ```bash
  http-server -c-1
  ```

- Open browser and enter the url `http://127.0.0.1:8080/code/a01_plot_Google_sankey.html`

- For changing the data of the graph, you can

  - First, modify the data in `data/*.csv`. (e.g. the value in `second_third.csv` define the connection between the second and third level)

  - Run `a03_apply_change_of_weights.py` by

    ```
    cd code
    python a03_apply_change_of_weights.py
    ```

    This will map your change to the `links.json` file, which is the actual data source of the sankey graph. 

  - Open `http://127.0.0.1:8080/code/a01_plot_Google_sankey.html` again and you will see the change.

