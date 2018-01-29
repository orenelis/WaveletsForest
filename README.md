# WaveletsForest
The wavelet decomposition of a Random Forest code provides a sparse approximation of regression or classification high dimensional function at various levels of details. The given the users setting and data, it provides analysis in the form of cross validation or prediction of testing set, given a testing set.
# Input
The input for the code is in the form of files for the training set – one file for the domain and one for the response variable – and example for the structure of the data format can be found at the folder DB sample
# Execution 
The code is implemented on C# .Net4.5. and the source code is available in the source code folder. In addition, there is provided an execution that can be called from other scripts such as python or R.
-note that for measuring Besov Smoothness, one could use the code at https://github.com/orenelis/BesovSmoothness
# Setting 
•In the I.O section write the path of your db and the path where you want to place the analysis results. 
•The Script config contains different options for running an analyzing the WF. 
•Run parallel checkbox enables a parallel mode for the implementation.
•Run RF pruning – provides comparative RF with pruning strategy tha is described in the article and could be viewed at the code.
•Run RF – is mandatory.
•Fold cross validation – use it in CV mode and denote how nany folds in the text box area.
•Threshold wavelets enable prediction with specific threshold is denoted + text box to insert its value.
•Use classification checkbox – to determine the error evaluation method (RMSE or accuracy).
•The use estimate __ - enable different kind of estimations and produce different text files in the result path.
•The parameter settings is:
oNumber of trees 
oN features RF – the hyper parameter value (you can type explicit value or use ‘sqrt’ or ‘all’ for the classic hyper parameter values)
oBagging (0.8 is 80% bagging)
oError type in estimation :”2” for regression “0” for classification 
oThe rest of the parametes should be fixed to the values descrbed in the image
# Output
For K fold cross validation, the code creates k folders and saves the results in them. The main results are the error provided by adding more trees on the testing set (to evaluate the accuracy) as well on the training set (to evaluate the weak type smoothness of the underlying function). The decision trees are also saved in a text format.  
 # Other
The code saves a file with config.txt under C folder that is read on runtime as a cash for the user’s settings 


