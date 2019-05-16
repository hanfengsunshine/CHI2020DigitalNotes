#!/usr/bin/env python
__author__ = "kelaxon"
__github__ = "https://github.com/Kelaxon"

import pandas as pd

def link_generator(df):
    for i, row in df.iterrows():
        for j, cell in row.iteritems():
            if cell==1:
                yield dict(source=i, target=j, value=1)

links = pd.DataFrame()
# change this to modify different level of the sankey graph
for file in ['first_second', 'second_third', 'third_fourth', 'fourth_fifth', 'fifth_sixth']:  
    csv_file = pd.read_csv('../data/%s.csv'%file, index_col=0)
    links = pd.concat([links, pd.DataFrame(list(link_generator(csv_file)))])
links.to_json(orient='values', path_or_buf='../data/links.json')
print("All changes has been applied.")
print("http://127.0.0.1:8080/code/a01_plot_Google_sankey.html")