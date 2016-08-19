function distance = manhattanDistance(vector1, vector2)

vector_size = size(vector1, 2);
vector_sum = 0;
cnt = 0;
for i=1:vector_size
    vector_sum = vector_sum + abs(vector1(i) - vector2(i));
    if vector1(i) == 1
        cnt = cnt + 1;
    elseif vector2(i) == 1
        cnt = cnt + 1;
    end
end

%number of ones (number of features/metrics) in each vector (job/task)
%numberOfOnesVector1 = nnz(vector1);
%numberOfOnesVector2 = nnz(vector2);
%maximum number of features between the two vectors (jobs)
%maxNumberOfOnes = max(numberOfOnesVector1, numberOfOnesVector2);

%this is the similarity, not the distance. 
%We divide the difference between the two vectors (jobs) by the maximum number of features between the two vectors (jobs)
distance = 1 -( vector_sum / cnt );